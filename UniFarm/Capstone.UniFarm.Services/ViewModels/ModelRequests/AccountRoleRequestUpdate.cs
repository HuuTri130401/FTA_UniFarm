using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record AccountRoleRequestUpdate
{
    public Guid Id { get; set; }
    public Guid? AccountId { get; set; }
    public Guid? StationId { get; set; }
    public Guid? CollectedHubId { get; set; }
    public Guid? FarmHubId { get; set; }
    [StringLength(100)]
    public string? Status { get; set; }
}