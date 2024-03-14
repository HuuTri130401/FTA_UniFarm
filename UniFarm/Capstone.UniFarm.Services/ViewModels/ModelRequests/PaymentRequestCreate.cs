using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record PaymentRequestCreate
{
    public Guid WalletId { get; set; }
    public decimal? Amount { get; set; }
    
    [RegularExpression("^(Deposit|Withdraw)$", ErrorMessage = "Type can be either Deposit or Withdraw.")]
    public string Type { get; set; }
    
    [RegularExpression("^(Success|Failure)$", ErrorMessage = "Status can be either Success or Failure.")]
    public string? Status { get; set; }
}