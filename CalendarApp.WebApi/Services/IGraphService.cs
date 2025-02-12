using CalendarApp.WebApi.Models;

namespace CalendarApp.WebApi.Services;

public interface IGraphService
{
    Task<List<User>> GetUsersStartingWith(List<string> searchTerms);
    Task<List<User>> GetUsersByEmails(List<string> emails);
    Task<List<Schedule>> GetUsersAvailability(AvailabilityRequest request);
    Task<MeetingResponse> CreateMeeting(CreateMeetingRequest request);
    Task<MeetingResponse> UpdateMeeting(UpdateMeetingRequest request);
    Task CancelMeeting(CancelMeetingRequest request);
}
