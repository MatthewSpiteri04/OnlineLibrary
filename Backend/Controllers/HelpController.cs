using System.Diagnostics;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    public class HelpController : ControllerBase
    {

        HelpService _helpService = new HelpService();

        [HttpGet]
        [Route("api/help")]

        public List<HelpDetails> GetHelpDetails()
        {
            return _helpService.getHelpDetails();
        }

        [HttpGet]
        [Route("api/help/{search}")]

        public List<HelpDetails> GetHelpDetailsAfterSearch(string search)
        {
            return _helpService.getHelpDetailsAfterSearch(search);
        }


        [HttpGet]
        [Route("api/help/answer/{questionId}")]
        public HelpDetails GetHelpAnswer(int questionId)
        {
            return _helpService.getHelpAnswer(questionId);
        }


    }
}
