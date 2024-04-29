using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Backend.Controllers
{
	public class SecurityController : ControllerBase
	{
		SecurityService _securityService = new SecurityService();

        [HttpPut]
        [Route("api/Update/UserInfo")]
        public User UpdateLoginInfo([FromBody] UpdateRequest request)
        {
            return _securityService.updateUserInfo(request);

        }

    }
}