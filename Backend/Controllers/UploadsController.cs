using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class UploadsController : ControllerBase
    {
        [HttpPost]
        [Route("api/Upload/File")]
        public async Task<IActionResult> UploadPdf()
        {
            var file = Request.Form.Files[0]; // Assuming only one file is uploaded

            if (file != null && file.Length > 0)
            {
                // Specify the directory where you want to save the uploaded PDFs
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                // If the directory doesn't exist, create it
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate a unique file name for the uploaded PDF
                var fileName = Path.Combine(uploadPath, Path.GetRandomFileName());

                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return Ok("PDF uploaded successfully.");
            }

            return BadRequest("No file uploaded.");
        }
    }
}
