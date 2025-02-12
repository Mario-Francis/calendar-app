using CalendarApp.WebApi.Models;
using CalendarApp.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController(IGraphService graphService) : ControllerBase
    {
        private readonly IGraphService _graphService = graphService;

        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckAvailability(AvailabilityRequest request)
        {
            var _schedules = await _graphService.GetUsersAvailability(request);
            return Ok(_schedules);
        }

        [HttpPost("events/create")]
        public async Task<IActionResult> CreateMeeting(CreateMeetingRequest request)
        {
            var _response = await _graphService.CreateMeeting(request);
            return Ok(_response);
        }

        [HttpPatch("events/update")]
        public async Task<IActionResult> UpdateMeeting(UpdateMeetingRequest request)
        {
            var _response = await _graphService.UpdateMeeting(request);
            return Ok(_response);
        }

        [HttpDelete("events/cancel")]
        public async Task<IActionResult> CancelMeeting(CancelMeetingRequest request)
        {
            await _graphService.CancelMeeting(request);
            return Ok();
        }
    }
}
