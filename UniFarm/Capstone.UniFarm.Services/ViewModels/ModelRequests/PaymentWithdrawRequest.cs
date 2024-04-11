using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record PaymentWithdrawRequest
{
    [Required]
    public string BankName { get; set; }
    
    [Required]
    public string BankOwnerName { get; set; }
    
    [Required]
    public string BankAccountNumber { get; set; }
    
    [Required]
    [Range(1000, int.MaxValue, ErrorMessage = "Số tiền rút phải lớn hơn 1000")]
    public int Amount { get; set; }
    public string? Note { get; set; }
}


