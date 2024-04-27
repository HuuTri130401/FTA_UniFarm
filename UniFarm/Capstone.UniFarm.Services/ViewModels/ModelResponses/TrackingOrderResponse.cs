using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record TrackingOrderResponse
{
    public string? Title { get; set; }
    public EnumConstants.DeliveryStatus OrderStatus { get; set; }
    public DateTime? UpdatedAt { get; set; }
}