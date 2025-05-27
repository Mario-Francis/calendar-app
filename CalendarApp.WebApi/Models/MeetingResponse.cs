namespace CalendarApp.WebApi.Models;

public record MeetingResponse
{
    public string? Id { get; init; }
    public string? Subject { get; init; }
    public string? MeetingLink { get; init; }

    public override string ToString()
    {
        return $"Meeting Id to be used for update/cancellation is: {Id},\nSubject: {Subject}, \nMeetingLink: {MeetingLink}";
    }
}
