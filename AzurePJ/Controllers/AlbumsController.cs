using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using AzurePJ.DbContexts;
using AzurePJ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Sas;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AzurePJ.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly DbToDoListContext _context;
        private readonly BlobService _blobService;

        public AlbumsController(DbToDoListContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Photos)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var model = albums.Select(a => new AlbumViewModel
            {
                Id = a.Id,
                Name = a.Name,
                CoverUrl = a.Photos != null && a.Photos.Any()
                           ? _blobService.GenerateSasUrl("thumbnails", a.Photos.First().ThumbnailPath)
                           : null,
                CreatedAt = a.CreatedAt
            }).ToList();

            return View(model);
        }


        public IActionResult Create()
        {
            var photos = _context.Photos
                .OrderByDescending(p => p.UploadedAt)
                .ToList();

            var groupedPhotos = photos
                .GroupBy(p => p.UploadedAt.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new PhotoGroupViewModel
                {
                    Date = g.Key,
                    Photos = g.Select(p => new PhotoViewModel
                    {
                        Id = p.Id,
                        ThumbnailUrl = _blobService.GenerateSasUrl("thumbnails", p.ThumbnailPath),
                        OriginalUrl = _blobService.GenerateSasUrl("mengmeng", p.OriginalPath),
                        Title = p.Title ?? ""
                    }).ToList()
                }).ToList();

            return View(groupedPhotos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, List<int> selectedPhotoIds, List<IFormFile> localFiles)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "アルバム名は必須です。");

                // 返回照片列表
                var photos = _context.Photos.OrderByDescending(p => p.UploadedAt).ToList();
                var groupedPhotos = photos
                    .GroupBy(p => p.UploadedAt.Date)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new PhotoGroupViewModel
                    {
                        Date = g.Key,
                        Photos = g.Select(p => new PhotoViewModel
                        {
                            Id = p.Id,
                            ThumbnailUrl = _blobService.GenerateSasUrl("thumbnails", p.ThumbnailPath),
                            OriginalUrl = _blobService.GenerateSasUrl("mengmeng", p.OriginalPath),
                            Title = p.Title ?? ""
                        }).ToList()
                    }).ToList();

                return View(groupedPhotos);
            }

            var album = new Album { Name = name, CreatedAt = DateTime.UtcNow };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var newPhotos = new List<Photo>();

            // 添加已有照片
            if (selectedPhotoIds?.Any() == true)
            {
                foreach (var pid in selectedPhotoIds)
                    _context.AlbumPhotos.Add(new AlbumPhoto { AlbumId = album.Id, PhotoId = pid });
            }

            // 上传本地文件
            if (localFiles?.Any() == true)
            {
                var uploadTasks = localFiles.Select(async file =>
                {
                    var guid = Guid.NewGuid();
                    var ext = Path.GetExtension(file.FileName);

                    var originalBlobName = $"{name}/original/{guid}{ext}";
                    var thumbnailBlobName = $"{name}/thumbnails/{guid}.jpg";

                    // 上传原图
                    using var stream = file.OpenReadStream();
                    await _blobService.UploadAsync("mengmeng", originalBlobName, stream, file.ContentType);

                    // 上传缩略图
                    using var thumbStream = file.OpenReadStream();
                    await _blobService.UploadThumbnailAsync("thumbnails", thumbnailBlobName, thumbStream, 200);

                    var photo = new Photo
                    {
                        AlbumId = album.Id,
                        Title = Path.GetFileNameWithoutExtension(file.FileName),
                        OriginalPath = originalBlobName,
                        ThumbnailPath = thumbnailBlobName,
                        UploadedAt = DateTime.UtcNow
                    };

                    newPhotos.Add(photo);
                });

                await Task.WhenAll(uploadTasks);

                _context.Photos.AddRange(newPhotos);
                await _context.SaveChangesAsync();

                foreach (var photo in newPhotos)
                    _context.AlbumPhotos.Add(new AlbumPhoto { AlbumId = album.Id, PhotoId = photo.Id });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Folder(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
                return NotFound();

            var groupedPhotos = album.Photos
                .OrderByDescending(p => p.UploadedAt)
                .GroupBy(p => p.UploadedAt.Date)
                .Select(g => new PhotoGroupViewModel
                {
                    Date = g.Key,
                    Photos = g.Select(p => new PhotoViewModel
                    {
                        Id = p.Id,
                        ThumbnailUrl = _blobService.GenerateSasUrl("thumbnails", p.ThumbnailPath),
                        OriginalUrl = _blobService.GenerateSasUrl("mengmeng", p.OriginalPath),
                        Title = p.Title ?? ""
                    }).ToList()
                }).ToList();

            ViewBag.AlbumName = album.Name;

            return View(groupedPhotos);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
                return NotFound();

            // 删除 Blob 里的文件
            foreach (var photo in album.Photos)
            {
                if (!string.IsNullOrEmpty(photo.OriginalPath))
                    await _blobService.DeleteAsync("mengmeng", photo.OriginalPath);

                if (!string.IsNullOrEmpty(photo.ThumbnailPath))
                    await _blobService.DeleteAsync("thumbnails", photo.ThumbnailPath);
            }

            // 删除数据库里的照片
            _context.Photos.RemoveRange(album.Photos);

            // 删除相册
            _context.Albums.Remove(album);

            //AlbumPhotosを削除

            var albumPhoto = await _context.AlbumPhotos.FirstOrDefaultAsync(ap => ap.AlbumId == album.Id);

            if (albumPhoto != null)
            {
                _context.AlbumPhotos.Remove(albumPhoto);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



    }
}
