using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record TransferRequestUpdate
{
    
    public Guid Id { get; set; }
    
    [Required]
    [RegularExpression("^(Received|NotReceived)$", ErrorMessage = "Status must be 'Received' or 'NotReceived'")]
    public string Status { get; set; }
    
    public string? NoteReceived { get; set; }
    
}