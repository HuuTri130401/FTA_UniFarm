using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record CollectedHubRequestUpdate
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Address { get; set; }
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status can be either 'Active' or 'Inactive'.")]
    public string? Status { get; set; }
}