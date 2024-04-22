using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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

		[HttpPost]
		[Route("api/Categories/AddCategory")]
		public IActionResult CreateCategory([FromBody] CategoryAttributes request)
		{
            IActionResult response;
			bool validAttributeList = true;
            HashSet<string> uniqueAttributesRequest = new HashSet<string>();
            foreach (Attributes attrb in request.Attributes)
            {
                if (uniqueAttributesRequest.Contains(attrb.Name))
                {
                    validAttributeList = false;
                }
                else
                {
                    uniqueAttributesRequest.Add(attrb.Name);
                }
            }

            bool validAttributes = _categoryService.checkValidAttributes(request.Attributes);
			bool validCategory = _categoryService.checkValidCategories(request.CategoryName);
			bool validUser = _categoryService.checkValidUser(request.UserId);

			if (validAttributes && validCategory && validUser && validAttributeList)
			{
				int categoryId = _categoryService.createCategory(request.CategoryName);

				List<int> attributeId = new List<int>();
				foreach (var attribute in request.Attributes)
				{
					attributeId.Add(_categoryService.createAttributes(attribute));
				}

				_categoryService.createCategoryAttributes(categoryId, attributeId);
				response = Ok(new { message = "Success!" });

			}
            else if (!validAttributeList)
            {
                response = BadRequest(new { message = "Duplicate Attributes In List" });
            }
            else if (!validAttributes)
			{
				response = BadRequest(new {message = "Duplicate Attribute"});
			}
			else if (!validCategory)
			{
				response = BadRequest(new { message = "Duplicate Category" });
			}
			else if (!validUser)
			{
				response = Unauthorized(new { message = "User Cannot Manage Categories" });
			}
			else
			{
				response = BadRequest(new { message = "Unknown Error Occurred " });
			}
			return response;

		}

		
	}
}
