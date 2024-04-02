namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record WalletResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public decimal? Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Status { get; set; }
    public AboutMeResponse.CustomerResponseSimple? Account { get; set; } = null!;
}