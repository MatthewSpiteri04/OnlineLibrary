using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
	public class CategoriesController : ControllerBase
	{
		CategoryService _categoryService = new CategoryService();

		[HttpGet]
		[Route("api/Categories/AttributeTypes")]

		public List<AttributeTypes> GetAttributeTypes()
		{
			return _categoryService.getAttributeTypes();
		}

		[HttpGet]
		[Route("api/Categories/GetAttributes")]

		public List<Attributes> GetAttributes()
		{
			return _categoryService.getAttributes();
		}

		[HttpPost]
		[Route("api/Categories/AddCategory")]
		public IActionResult CreateCategory([FromBody] CategoryAttributes request) { 
			 if (request == null || request.Attributes == null)
				{
					// Handle the case where request or its Attributes property is null
					return BadRequest("Request or Attributes are null");
				}
		
			bool validAttributes = _categoryService.checkValidAttributes(request.Attributes);
			bool validCategory = _categoryService.checkValidCategories(request.CategoryName);
			bool validUser = _categoryService.checkValidUser(request.UserId);

			IActionResult response;
			if (validCategory)
			{
				int categoryId = _categoryService.createCategory(request.CategoryName);

				List<int> attributeId = new List<int>();

				foreach (var attribute in request.Attributes)
				{
					if (!attribute.ListView)
					{
						attributeId.Add(_categoryService.createAttributes(attribute));
					}

				}

				 _categoryService.createCategoryAttributes(categoryId, attributeId);
				response = Ok(new { message = "Success!" });

			}
			
			else if (!validCategory)
			{
				response = BadRequest(new { message = "Duplicate Category" });
			}
			
			else
			{
				response = BadRequest(new { message = "Unknown Error Occurred " });
			}
			return response;

		}

		
	}
}