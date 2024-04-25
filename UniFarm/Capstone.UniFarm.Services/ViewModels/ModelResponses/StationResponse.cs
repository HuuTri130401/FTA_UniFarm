using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record StationResponse
{
    public Guid Id { get; set; }
    public Guid AreaId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Address { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Status { get; set; }
    public Area? Area { get; set; }

    public record StationResponseSimple
    {
        public Guid Id { get; set; }
        public Guid AreaId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Status { get; set; }
    }
    
    public record StationDashboardResponse
    {
        public int TotalOrder { get; set; }
        public int TotalOrderOnTheWayToStation { get; set; }
        public int TotalOrderAtStation { get; set; }
        public int TotalOrderStationNotReceived { get; set; }
        public int TotalOrderReadyForPickup { get; set; }
        public int TotalOrderPickedUp { get; set; }
        public int TotalOrderExpired { get; set; }
        public int TotalOrderStaffHandled { get; set; }
        public int TotalTransferPending { get; set; }
        public int TotalTransferReceived { get; set; }
        public int TotalTransferNotReceived { get; set; }
        public int TotalTransferProcessed { get; set; }
    }
}
