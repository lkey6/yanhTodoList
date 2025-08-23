using AzurePJ.DbContexts;
using AzurePJ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzurePJ.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly DbToDoListContext _context;

        public AlbumsController(DbToDoListContext context)
        {
            _context = context;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Photos)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(albums);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album)
        {
            if (ModelState.IsValid)
            {
                album.CreatedAt = DateTime.UtcNow;
                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
                TempData["Success"] = "アルバムが作成されました！";
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/UploadPhoto/5
        public async Task<IActionResult> UploadPhoto(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null) return NotFound();

            ViewBag.AlbumName = album.Name;
            ViewBag.AlbumId = album.Id;
            ViewBag.Message = "ここにあなたの大切な思い出の写真をアップロードしましょう♡";
            return View();
        }

        // POST: Albums/UploadPhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(PhotoUploadViewModel model, int albumId)
        {
            if (model.AlbumId == 0)
                model.AlbumId = albumId;

            if (!ModelState.IsValid)
            {
                ViewBag.AlbumName = (await _context.Albums.FindAsync(model.AlbumId))?.Name;
                return View(model);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(model.PhotoFile.FileName);
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/albums", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await model.PhotoFile.CopyToAsync(stream);
            }

            var photo = new Photo
            {
                AlbumId = model.AlbumId,
                Title = model.Title,
                Description = model.Description,
                Url = "/images/albums/" + fileName,
                UploadedAt = DateTime.UtcNow
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            TempData["Success"] = "写真をアップロードしました♡";
            return RedirectToAction(nameof(Index));
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null) return NotFound();

            return View(album);
        }
    }
}
