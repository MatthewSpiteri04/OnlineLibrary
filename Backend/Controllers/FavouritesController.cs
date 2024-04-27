using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Backend.Controllers
{
	public class FavouritesController : ControllerBase
	{
		FavouritesService _favouritesService = new FavouritesService();

		[HttpPost]
		[Route("api/Get/Favourites")]
		public List<Documents> GetFavourites([FromBody] FavouriteSearchRequest request)
		{
            if (request.SearchString == null || request.SearchString == "")
            {
                return _favouritesService.getFavourites(request.UserId);
            }
            else
            {
                return _favouritesService.getFavouritesBySearch(request);
            }
        }
    }
}