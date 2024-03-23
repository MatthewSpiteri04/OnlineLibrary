using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class UsersController : ControllerBase
    {
        UserService _userService = new UserService();

        [HttpGet]
        [Route("api/User/Roles")]
        public IEnumerable<string> Get()
        {
            return _userService.getRoles();
        }

        [HttpGet]
        [Route("api/User/Exists/{username}")]
        public bool UsernameCheck(string username)
        {
            return _userService.doesUserExist(username);
        }
    }
}
