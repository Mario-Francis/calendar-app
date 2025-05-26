namespace CalendarApp.WebApi.Models;

public record User
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Email: {Email}, FirstName: {FirstName}, LastName: {LastName}, DisplayName: {DisplayName}";
    }
}
