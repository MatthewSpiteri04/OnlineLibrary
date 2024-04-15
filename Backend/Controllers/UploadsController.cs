using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class UploadsController : ControllerBase
    {
        UploadService _uploadService = new UploadService();

        [HttpPost]
        [Route("api/Upload/File")]
        public async Task<IActionResult> UploadPdf([FromForm] UploadRequest request)
        {            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var fileName = Path.Combine(uploadPath, request.Title + "_" + request.UserId + ".pdf");
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            UploadDatabaseRequest database_request = new UploadDatabaseRequest() 
            { 
                CategoryId = 1,
                Title = request.Title,
                Language = request.Language,
                UploadDate = DateTime.Now,
                PublicAccess = request.PublicAccess,
                DocumentLocation = fileName
            };

            _uploadService.SaveUploadedFile(database_request);

            return Ok("PDF uploaded successfully.");
        }

    }
}
