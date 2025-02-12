namespace CalendarApp.WebApi.Models;

public record ScheduleItem
{
    public DateTimeOffset? Start { get; init; }
    public DateTimeOffset? End { get; init; }
    public ScheduleStatus Status { get; init; }
    public string? Subject { get; init; }
}
