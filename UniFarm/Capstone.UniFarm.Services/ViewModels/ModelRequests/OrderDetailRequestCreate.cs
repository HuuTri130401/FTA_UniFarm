namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record OrderDetailRequestCreate
{
    public Guid ProductItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Unit { get; set; }
    public decimal TotalPrice { get; set; }
}