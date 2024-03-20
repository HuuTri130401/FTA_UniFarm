namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record ApartmentStationRequestCreate
{
    public Guid StationId { get; set; }
    public Guid ApartmentId { get; set; }
    public bool? IsDefault { get; set; }
}