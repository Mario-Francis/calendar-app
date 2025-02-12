namespace CalendarApp.WebApi.Models;

public class AzureAdSettings
{
    public required string Instance { get; set; }
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public string[] Scopes { get; set; } = [];
}
