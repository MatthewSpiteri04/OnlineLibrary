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

		public List<AttributeTypes> GetAttributeTypes()
		{
			return _categoryService.getAttributeTypes();
		}

		[HttpPost]
		[Route("api/Categories/AddCategory")]
		public Categories CreateCategory([FromBody] Categories category)
		{
			return _categoryService.createCategory(category);
		}

		[HttpPost]
		[Route("api/Attributes/AddAttributes")]
		public Attributes CreateAttrbutes([FromBody] Attributes attribute)
		{
			return _categoryService.createAttributes(attribute);
		}


	}
}
