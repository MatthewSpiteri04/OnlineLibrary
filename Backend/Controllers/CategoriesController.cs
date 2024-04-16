using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
	public class CategoriesController : Controller
	{
		CategoryService _categoryService = new CategoryService();

		[HttpGet]
		[Route("api/Categories/AttributeTypes")]

		public List<string> GetAttributeTypes()
		{
			return _categoryService.getAttributeTypes();
		}


	}
}
