using System.Diagnostics;
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
    }
}
