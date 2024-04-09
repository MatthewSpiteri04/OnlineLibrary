using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class UsersController : ControllerBase
    {
        UserService _userService = new UserService();

        [HttpGet]
        [Route("api/User/Roles/{id}")]
        public Permissions GetRoles(int id)
        {
            return _userService.getRoles(id);
        }

        [HttpGet]
        [Route("api/User/Exists/{login}")]
        public bool UsernameCheck(string login)
        {
            return _userService.doesUserExist(login);
        }

        [HttpPost]
        [Route("api/User/Login")]
        public User Login([FromBody] LoginData loginInfo)
        {
            Debug.WriteLine(loginInfo);
            return _userService.loginUser(loginInfo);
        }


        [HttpPost]
        [Route("api/User/Add")]
        public User CreateUser([FromBody] User user)
        {
            return _userService.createUser(user);
        }
    }
}
