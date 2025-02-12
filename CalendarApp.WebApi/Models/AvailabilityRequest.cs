namespace CalendarApp.WebApi.Models;

public record AvailabilityRequest
{
    /// <summary>
    /// The email address of the user who is initiating the request.
    /// </summary> 
    public string? InitiatingUserEmail { get; set; }
    /// <summary>
    /// A list of user email addresses (or identifiers) for which to retrieve availability.
    /// </summary>
    public List<string> Emails { get; set; } = [];

    /// <summary>
    /// The start time for the availability window.
    /// </summary>
    public DateTimeOffset? StartTime { get; set; }

    /// <summary>
    /// The end time for the availability window.
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }

    /// <summary>
    /// The time zone (e.g., "UTC" or "Pacific Standard Time").
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// The interval (in minutes) for dividing the availability slots.
    /// </summary>
    public int AvailabilityViewInterval { get; set; } = 30;
}
