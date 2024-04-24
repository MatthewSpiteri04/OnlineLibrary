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

		[HttpGet]
		[Route("api/Get/Favourites/{id}")]
		public List<Documents> GetFavourites(int id)
		{
			return _favouritesService.getFavourites(id);
		}


    }
}