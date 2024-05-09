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

		[HttpGet]
		[Route("api/Categories/GetAttributes")]

		public List<AttributesWithTypeName> GetAttributes()
		{
			return _categoryService.getAttributes();
		}

        [HttpGet]
        [Route("api/Categories/GetCategories/{categoryId}")]

        public EditCategoryAttributeRequest GetCategories(int categoryId)
        {
            return _categoryService.getCategories(categoryId);
        }

        [HttpDelete]
        [Route("api/Delete/Category/{id}")]
        public IActionResult DeleteCategory(int id)
		{
			if (_categoryService.deleteCategory(id))
			{
				return Ok();
			}
			else
			{
				return BadRequest(new { Title = "Delete Failed", Message = "This category has documents related to it. Make sure this category isn't being used first."});
			}
		}

        [HttpDelete]
        [Route("api/Delete/Attribute/{id}")]
        public IActionResult DeleteAttribute(int id)
        {
            if (_categoryService.deleteAttribute(id))
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Title = "Delete Failed", Message = "This attribute has documents related to it. Make sure this attribute isn't being used first." });
            }
        }


        [HttpPost]
		[Route("api/Categories/AddCategory")]
		public IActionResult CreateCategory([FromBody] CategoryAttributes request)
		{
			if (request == null || request.Attributes == null)
			{
				return BadRequest("Request or Attributes are null");
			}

			IActionResult response;
			bool validAttributeList = true;
			HashSet<string> uniqueAttributesRequest = new HashSet<string>();
			foreach (Attributes attrb in request.Attributes)
			{
				if (uniqueAttributesRequest.Contains(attrb.Name) && (attrb.Name != ""))
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
					if (!attribute.ListView)
					{
						attributeId.Add(_categoryService.createAttributes(attribute));
					}
					else
					{

						attributeId.Add((int)attribute.Id);
					}

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
				response = BadRequest(new { message = "Duplicate Attribute" });
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
        [HttpPut]
        [Route("api/Update/Category")]
        public EditCategoryAttributeRequest UpdateCategory([FromBody] EditCategoryAttributeRequest request)
        {
            return _categoryService.updateCategory(request);

        }

    }
}