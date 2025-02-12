using CalendarApp.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IGraphService graphService) : ControllerBase
    {
        private readonly IGraphService _graphService = graphService;

        [HttpGet("search/starts-with")]
        public async Task<IActionResult> GetUsersStartingWith([FromQuery] List<string> searchTerms)
        {
            var _users = await _graphService.GetUsersStartingWith(searchTerms);
            return Ok(_users);
        }

        [HttpGet("search/emails-in")]
        public async Task<IActionResult> GetUsersByEmails([FromQuery] List<string> emails)
        {
            var _users = await _graphService.GetUsersByEmails(emails);
            return Ok(_users);
        }
    }
}
