using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("api/User/Test")]
        public IEnumerable<string> Get()
        {
            return new string[] { };
        }
    }
}
