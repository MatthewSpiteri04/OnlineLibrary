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

        [HttpDelete]
        [Route("api/Delete/User/{id}")]
        public IActionResult DeleteUser(int id)
        {
            bool userHaveDocuments = _securityService.getUserDocuments(id);

            if (userHaveDocuments)
            {
                User headmaster = _securityService.searchForFileHandler(id);

                if (headmaster.Id <= 0)
                {
                    return BadRequest(new { Title = "User Cannot Be Deleted", Message = "No other eligible user can handle your documents" });
                }
                else
                {
                    _securityService.updateDocumentsAndDeleteUser(headmaster, id);
                    return Ok();
                }
            }
            else
            {
                _securityService.deleteUser(id);
                return Ok();
            }
        }
    }
}