namespace CalendarApp.WebApi.Models;

public record MeetingResponse
{
    public string? Id { get; init; }
    public string? Subject { get; init; }
    public string? MeetingLink { get; init; }
}
