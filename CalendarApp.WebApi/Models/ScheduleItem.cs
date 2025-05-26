namespace CalendarApp.WebApi.Models;

public record ScheduleItem
{
    public DateTimeOffset? Start { get; init; }
    public DateTimeOffset? End { get; init; }
    public ScheduleStatus Status { get; init; }
    public string? Subject { get; init; }

    public override string ToString()
    {
        return $"Start: {Start}, End: {End}, Status: {Status}, Subject: {Subject}";
    }
}
