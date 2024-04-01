using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record UpdateOrderStatus
{

    public record UpdateOrderStatusByTransfer
    {
        public Guid TransferId { get; init; }
        public Guid OrderId { get; init; }
        public Guid StationId { get; init; }
        public EnumConstants.StationStaffUpdateOrderStatus DeliveryStatus { get; init; }
    }
}