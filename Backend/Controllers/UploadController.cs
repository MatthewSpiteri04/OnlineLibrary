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
                    CategoryId = request.CategoryId,
                    Title = request.Title,
                    LanguageId = request.LanguageId,
                    UploadDate = DateTime.Now,
                    PublicAccess = request.PublicAccess,
                    DocumentLocation = fileName
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

        [HttpGet]
        [Route("api/Get/Attributes/{categoryId}")]
        public List<AttributeRequest> GetAttributes(int categoryId)
        {
            return _uploadService.getAttributes(categoryId);
        }

    }
}
