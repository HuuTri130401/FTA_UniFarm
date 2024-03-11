using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record AccountRoleRequest
{
    public Guid? Id { get; set; }
    public Guid? AccountId { get; set; }
    public Guid? StationId { get; set; } = null;
    public Guid? CollectedHubId { get; set; } = null;
    public Guid? FarmHubId { get; set; } = null;
    [StringLength(100)]
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status can be either 'Active' or 'Inactive'.")]
    public string? Status { get; set; }
}