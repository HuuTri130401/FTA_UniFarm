namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record CheckoutRequest
{
    public Guid OrderId { get; set; }
    public List<Guid> OrderDetailIds { get; set; }
}