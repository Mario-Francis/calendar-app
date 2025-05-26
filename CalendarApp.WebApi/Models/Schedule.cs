namespace CalendarApp.WebApi.Models;

public record Schedule
{
    public string? ScheduleId { get; init; }
    public string? AvailabilityView { get; init;}
    public string? Error { get; init;}
    public ScheduleItem[]? ScheduleItems { get; init; }
    public WorkingHours? WorkingHours { get; init; }

    public override string ToString()
    {
        var items = ScheduleItems != null ? string.Join("; ", ScheduleItems.Select(i => i.ToString())) : "None";
        return $"ScheduleId: {ScheduleId}, AvailabilityView: {AvailabilityView}, Error: {Error}, ScheduleItems: [{items}], WorkingHours: {WorkingHours}";
    }
}
