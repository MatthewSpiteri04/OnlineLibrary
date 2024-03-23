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
        public IEnumerable<string> GetRoles()
        {
            return _userService.getRoles();
        }

        [HttpGet]
        [Route("api/User/Exists/{username}")]
        public bool UsernameCheck(string username)
        {
            return _userService.doesUserExist(username);
        }

        [HttpGet]
        [Route("api/User/Login/{username}/{password}")]
        public User Login(string username, string password)
        {
            Login login = new Login();
            login.Username = username;
            login.Password = password;

            return _userService.loginUser(login);
        }


        [HttpPost]
        [Route("api/User/Add")]
        public User CreateUser([FromBody] User user)
        {
            return _userService.createUser(user);
        }
    }
}
