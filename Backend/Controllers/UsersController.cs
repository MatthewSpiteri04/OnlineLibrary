using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;

namespace Backend.Controllers
{
    public class UsersController : Controller
    {
        UserService _userService = new UserService();

        [HttpGet]
        [Route("api/User/Roles/{id}")]
        public List<string> GetRolePrivileges(int id)
        {
            return _userService.getRolePrivileges(id);
        }

        [HttpGet]
        [Route("api/User/Exists/{login}")]
        public bool UsernameCheck(string login)
        {
            return _userService.doesUserExist(login);
        }

        [HttpGet]
        [Route("api/User/UpdateRoles/{userId}/{newRole}")]

        //public IActionResult UpdateRole(string userId, string newRole)
       // {
            

        //}

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

        private readonly IEmailSender emailSender;

        public UsersController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        [HttpPost]
        [Route("api/User/Send")]
        public async Task<IActionResult> Index([FromBody] EmailRequestModel model)
        {
            var userId = model.UserId;
            var email = model.Email;
            var subject = model.Subject;
            var message = model.Message;

            var localhostUrl = "https://localhost:44311";
            var link = $"{localhostUrl}/api/User/UpdateRole?userId={userId}";

           
            var htmlMessageContent = $"Click <a href=\"{link}\">here</a> to update user roles";

            
            message += $"\n\n{htmlMessageContent}";
            await emailSender.SendEmailAsync(email,subject, message);
            return View();
          
        }

        [HttpGet]
        [Route("api/User/UpdateRole")]
        public IActionResult UpdateUserRole([FromQuery] int userId)
        {
            
            _userService.UpdateUserRole(userId);
            return Redirect("http://127.0.0.1:5500/index.html#!/home");
        }




    }
}
    

