using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    public class HomeController : ControllerBase
    {
        HomeService _homeService = new HomeService();


        [HttpGet]
        [Route("api/Get/Categories")]
        public List<CategoryRequest> GetCategories()
        {
            return _homeService.getCategories();
        }

        [HttpGet]
        [Route("api/Get/Languages")]
        public List<LanguageRequest> GetLanguages()
        {
            return _homeService.getLanguages();
        }
    }
}
