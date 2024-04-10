using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public Guid WalletId { get; set; }
        public string PaymentMethod { get; set; }
        public double Amount { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }

    public class VnPaymentRequestModel
    {
        public Guid WalletId { get; set; }
        public EnumConstants.PaymentMethod PaymentMethod { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    public record PaymentRequestCreateModel
    {
        public Guid? WalletId { get; init; }
        public EnumConstants.FromToWallet From { get; init; }
        public EnumConstants.FromToWallet To { get; init; }
        public decimal Amount { get; init; }
        public EnumConstants.PaymentMethod Type { get; init; }
    };
}