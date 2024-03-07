namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record CollectedHubRequestCreate
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Address { get; set; }
}