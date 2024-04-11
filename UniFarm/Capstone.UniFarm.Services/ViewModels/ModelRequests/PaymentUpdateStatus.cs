using System.ComponentModel.DataAnnotations;
using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record PaymentUpdateStatus
{
    public Guid Id { get; set; }
    
    [RegularExpression(@"^(SUCCESS|DENIED)$", ErrorMessage = "Status must be SUCCESS or DENIED")]
    public string Status { get; set; }
    
}