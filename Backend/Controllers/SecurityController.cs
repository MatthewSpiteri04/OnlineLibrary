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

        [HttpPut]
        [Route("api/Update/UserPassword")]
        public User UpdatePasswordInfo([FromBody] UpdateRequest request)
        {
            return _securityService.updateUserPassword(request);

        }

        [HttpDelete]
        [Route("api/Delete/User/{id}")]
        public IActionResult DeleteUser(int id)
        {
            bool userHaveDocuments = _securityService.getUserDocuments(id);
            User headmaster = _securityService.searchForFileHandler(id);

            if (userHaveDocuments && headmaster.Id > 0)
            {
                _securityService.updateDocumentsAndDeleteUser(headmaster, id);
                return Ok();
            }
            else if (!userHaveDocuments && headmaster.Id > 0)
            {
                _securityService.deleteUser(id);
                return Ok();
            }
            else
            {
                return BadRequest(new { Title = "User Cannot Be Deleted", Message = "No other eligible user can handle your documents" });
            }
        }
        [HttpDelete]
        [Route("api/Delete/UserDocuments/{id}")]
        public IActionResult DeleteUserAndDocuments(int id)
        {
            User headmaster = _securityService.searchForFileHandler(id);
            if (headmaster.Id > 0)
            {
                _securityService.deleteUserAndDocuments(id);
                return Ok();
            }
            else
            {
                return BadRequest(new { Title = "User Cannot Be Deleted", Message = "No other eligible user can handle your documents" });
            }
        }

    }
}