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
        [Route("api/User/Send/Student")]
        public async Task<IActionResult> Index([FromBody] EmailRequestModel model)
        {
            var userId = model.UserId;
            var subject = model.Subject;
            var message = model.Message;

            var localhostUrl = "https://localhost:44311";
            var link = $"{localhostUrl}/api/User/UpdateRole/Student?userId={userId}";

           
            var htmlMessageContent = $"Click <a href=\"{link}\">here</a> to update user roles";

            
            message += $"\n\n{htmlMessageContent}";
            var emails = _userService.GetEmailsByRoleId(4); 

            foreach (var email in emails)
            {
                await emailSender.SendEmailAsync(email, subject, message);
            }
            return Ok();
          
        }

        [HttpPost]
        [Route("api/User/Send/Lecturer")]
        public async Task<IActionResult> LecturerIndex([FromBody] EmailRequestModel model)
        {
            var userId = model.UserId;
            var subject = model.Subject;
            var message = model.Message;

            var localhostUrl = "https://localhost:44311";
            var link = $"{localhostUrl}/api/User/UpdateRole/Lecturer?userId={userId}";


            var htmlMessageContent = $"Click <a href=\"{link}\">here</a> to update user roles";


            message += $"\n\n{htmlMessageContent}";
            var emails = _userService.GetEmailsByRoleId(4);

            foreach (var email in emails)
            {
                await emailSender.SendEmailAsync(email, subject, message);
            }
            return Ok();

        }

        [HttpPost]
        [Route("api/User/Send/Librarian")]
        public async Task<IActionResult> LibrarianIndex([FromBody] EmailRequestModel model)
        {
            var userId = model.UserId;
            var subject = model.Subject;
            var message = model.Message;

            var localhostUrl = "https://localhost:44311";
            var link = $"{localhostUrl}/api/User/UpdateRole/Librarian?userId={userId}";


            var htmlMessageContent = $"Click <a href=\"{link}\">here</a> to update user roles";


            message += $"\n\n{htmlMessageContent}";
            var emails = _userService.GetEmailsByRoleId(5);

            foreach (var email in emails)
            {
                await emailSender.SendEmailAsync(email, subject, message);
            }
            return Ok();

        }

       



        [HttpGet]
        [Route("api/User/UpdateRole/Student")]
        public IActionResult UpdateToStudent([FromQuery] int userId)
        {
            
            _userService.UpdateToStudent(userId);
            return Redirect("http://127.0.0.1:5500/index.html#!/home");
        }

        [HttpGet]
        [Route("api/User/UpdateRole/Lecturer")]
        public IActionResult UpdateToLecturer([FromQuery] int userId)
        {

            _userService.UpdateToLecturer(userId);
            return Redirect("http://127.0.0.1:5500/index.html#!/home");
        }

        [HttpGet]
        [Route("api/User/UpdateRole/Librarian")]
        public IActionResult UpdateToLibrarian([FromQuery] int userId)
        {

            _userService.UpdateToLibrarian(userId);
            return Redirect("http://127.0.0.1:5500/index.html#!/home");
        }




    }
}
    

