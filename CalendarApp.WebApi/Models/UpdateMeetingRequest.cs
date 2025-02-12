namespace CalendarApp.WebApi.Models;

public record UpdateMeetingRequest
{
    public required string Id { get; init; }
    public required string InitiatingUserEmail { get; init; }
    public string? Subject { get; init; }
    public string? Body { get; init; }
    public DateTimeOffset? Start { get; init; }
    public DateTimeOffset? End { get; init; }
    public Attendee[]? Attendees { get; init; }
    public string? Location { get; init; }
    public bool? IsOnlineMeeting { get; init; }
}
