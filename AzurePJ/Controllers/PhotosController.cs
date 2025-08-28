using AzurePJ.DbContexts;
using AzurePJ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AzurePJ.Controllers
{
    public class PhotosController : Controller
    {
        private readonly DbToDoListContext _context;
        private readonly BlobService _blobService;

        public PhotosController(DbToDoListContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public IActionResult Index()
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
                        ThumbnailUrl = _blobService.GenerateSasUrl("thumbnails", p.ThumbnailPath),
                        OriginalUrl = _blobService.GenerateSasUrl("mengmeng", p.OriginalPath),
                        Title = p.Title ?? ""
                    }).ToList()
                }).ToList();

            return View(groupedPhotos);
        }

        public IActionResult Upload() => View();

        [HttpGet]
        public async Task<IActionResult> SelectPhotos(int albumId)
        {
            var photos = await _context.Photos
                .OrderByDescending(p => p.UploadedAt)
                .ToListAsync();

            ViewBag.AlbumId = albumId;
            return View(photos);
        }

    }
}
