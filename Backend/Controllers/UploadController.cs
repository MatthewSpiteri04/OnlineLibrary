using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    public class UploadController : ControllerBase
    {
        UploadService _uploadService = new UploadService();

        [HttpPost]
        [Route("api/Upload/File")]
        public async Task<IActionResult> UploadPdf([FromForm] UploadRequest request)
        {
            List<AttributeUploadRequest> attributes = JsonConvert.DeserializeObject<List<AttributeUploadRequest>>(request.AttributesListJSON);

            if (_uploadService.CanUserUpload(request.UserId))
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".mp3", ".wav", ".xlsx", ".pptx" }; // Add more extensions if needed

                var fileExtension = Path.GetExtension(request.File.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new {Title="File Not Supported", Message="This file type is not recognised. Upload has been stopped."});
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var fileName = Path.Combine(uploadPath, request.Title + "_" + request.UserId + fileExtension);
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                UploadDatabaseRequest database_request = new UploadDatabaseRequest()
                {
                    UserId = request.UserId,
                    CategoryId = request.CategoryId,
                    Title = request.Title,
                    LanguageId = request.LanguageId,
                    UploadDate = DateTime.Now,
                    PublicAccess = request.PublicAccess,
                    DocumentLocation = fileName,
                    FileExtension = fileExtension
                };

                int documentId = _uploadService.SaveUploadedFile(database_request);

                _uploadService.setDocumentAttribute(documentId, attributes);

                return Ok("Success");
            }
            else
            {
                return Unauthorized("User does not have upload priviledges");
            }
        }

        [HttpPost]
        [Route("api/Download/Document")]
        public IActionResult DownloadDocument([FromBody] Documents document)
        {
            if (!System.IO.File.Exists(document.DocumentLocation))
            {
                return NotFound("File not found");
            }

            var stream = new FileStream(document.DocumentLocation, FileMode.Open, FileAccess.Read);

            if (document.FileExtension == ".pdf")
            {
                return File(stream, "application/pdf");
            }
            else if (document.FileExtension == ".wav")
            {
                return File(stream, "audio/wav");
            }
            else if (document.FileExtension == ".mp3")
            {
                return File(stream, "audio/mpeg");
            }
            else if (document.FileExtension == ".doc")
            {
                return File(stream, "application/msword");
            }
            else if (document.FileExtension == ".docx")
            {
                return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
            else if (document.FileExtension == ".xlsx")
            {
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            else if (document.FileExtension == ".pptx")
            {
                return File(stream, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            }
            else if (document.FileExtension == ".txt")
            {
                return File(stream, "text/plain");
            }
            else
            {
                return BadRequest(new {Title = "File Extension Not Recognised", Message = "This file's extension is not recognised."});
            }
        }

        [HttpGet]
        [Route("api/Get/Attributes/{categoryId}")]
        public List<AttributesTypeRequest> GetAttributes(int categoryId)
        {
            return _uploadService.getAttributes(categoryId);
        }

    }
}
