using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AzureBlobStorage.Services;
using Microsoft.AspNetCore.Http;

namespace AzureBlobStorage.Controllers
{
    public class DemoController : Controller
    {
        private readonly IBlobService _blobService;

        public DemoController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var blob = await _blobService.ListBlobsAsync();

            return View(blob);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile files)
        {
            await _blobService.UploadContentBlobAsync(files, ModelState);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string blobName)
        {
            var data = await _blobService.GetBlobAsync(blobName);

            return File(data.Content, data.ContentType);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string blobName)
        {
            await _blobService.DeleteBlobAsync(blobName);

            return RedirectToAction("Index");
        }
    }
}
