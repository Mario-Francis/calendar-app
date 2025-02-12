namespace CalendarApp.WebApi.Models;

public record WorkingHours
{
    public DayOfWeek[]? DaysOfWeek { get; init; }
    public DateTimeOffset? StartTime { get; init; }
    public DateTimeOffset? EndTime { get; init; }
}
