namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record OrderDetailResponse
{
    public Guid ProductItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Title { get; set; }
    
    public string? ProductImage { get; set; }
}