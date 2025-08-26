using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using AzurePJ.DbContexts;
using AzurePJ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Sas;

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

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var folders = await _blobService.GetFoldersAsync();
            return View(folders);
        }

        public async Task<IActionResult> Folder(string name)
        {
            var images = await _blobService.GetImagesInFolderAsync(name);
            ViewBag.FolderName = name;
            return View(images);
        }
    }
}
