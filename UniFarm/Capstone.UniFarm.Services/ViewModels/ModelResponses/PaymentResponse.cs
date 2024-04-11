namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record PaymentResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public decimal Balance { get; set; }
    public decimal TransferAmount { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PaymentDay { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public string? BankName { get; set; }
    public string? BankOwnerName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? Code { get; set; }
    public string? Note { get; set; }
}