using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record OrderDetailResponseForCustomer
{
    public Guid OrderId { get; init; }
    public Guid ProductItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Unit { get; set; }
    public decimal TotalPrice { get; set; }
    public ProductItemResponseForCustomer? ProductItemResponse { get; set; }
}