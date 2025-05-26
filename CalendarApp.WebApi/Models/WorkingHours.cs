namespace CalendarApp.WebApi.Models;

public record WorkingHours
{
    public DayOfWeek[]? DaysOfWeek { get; init; }
    public DateTimeOffset? StartTime { get; init; }
    public DateTimeOffset? EndTime { get; init; }

    public override string ToString()
    {
        var days = DaysOfWeek != null ? string.Join(", ", DaysOfWeek) : "None";
        return $"DaysOfWeek: [{days}], StartTime: {StartTime}, EndTime: {EndTime}";
    }
}
