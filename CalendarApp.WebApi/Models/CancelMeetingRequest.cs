namespace CalendarApp.WebApi.Models;

public record CancelMeetingRequest
{
    public required string Id { get; init; }
    public required string InitiatingUserEmail { get; init; }
    public string? Comment { get; init; } = "Meeting cancelled.";
}
