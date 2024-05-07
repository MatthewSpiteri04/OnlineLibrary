using System.Diagnostics;
using System.Reflection.Metadata;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class DocumentsController : ControllerBase
    {
        DocumentsService _documentsService = new DocumentsService();

        [HttpPost]
        [Route("api/Get/Documents")]
        public List<Documents> GetDocuments([FromBody] DocumentRequestModel request)
        {
            if (request.UserId == null)
            {
                request.UserId = -1;
            }

            if (request.Search == null || request.Search == "") 
            {
               
                return _documentsService.getAllDocuments((int)request.UserId);
            }
            else
            {
                return _documentsService.getDocuments(request, (int)request.UserId);
            }

        }

        [HttpPost]
        [Route("api/Toggle/Favourite")]
        public IActionResult ToggleFavourite([FromBody] FavouriteRequest request)
        {
            _documentsService.toggleFavourite(request);

            return Ok();
        }

        [HttpPost]
        [Route("api/Get/MyUploads")]
        public List<Documents> GetMyUploads([FromBody] DocumentRequestModel request)
        {
            if (request.Search == null || request.Search == "")
            {
                return _documentsService.getMyUploads((int) request.UserId);
            }
            else
            {
                return _documentsService.getMyUploadsBySearch((int) request.UserId, request.Search);
            }
        }

        [HttpDelete]
        [Route("api/Delete/Document/{id}")]
        public IActionResult DeleteDocument(int id)
        {
            Documents document = _documentsService.getDocumentById(id);

            if (document == null)
            {
                return NotFound("Document not found");
            }

            try
            {
                System.IO.File.Delete(document.DocumentLocation);
                _documentsService.deleteDocument(id);
                return Ok("Document deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }

        [HttpGet]
        [Route("api/getDocument/{id}")]
        public DocumentWithAttribute GetDocument(int id)
        {
            return _documentsService.getDocumentsandAttributes(id);
        }

    }
}
