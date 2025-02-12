namespace CalendarApp.WebApi.Models;

public record Attendee
{
    public required string Email { get; init; }
    public required string Name { get; init; }
    public required bool IsRequired { get; init; } = false;
}
