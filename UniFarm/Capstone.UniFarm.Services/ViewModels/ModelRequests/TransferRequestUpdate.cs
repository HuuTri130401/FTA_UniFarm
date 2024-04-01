using System.ComponentModel.DataAnnotations;
using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record TransferRequestUpdate
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Status is required")]
    public EnumConstants.StationUpdateTransfer Status { get; set; }
    
    public string? NoteReceived { get; set; }
    
}