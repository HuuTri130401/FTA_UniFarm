namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record ApartmentStationResponse
{
    public Guid Id { get; set; }
    public Guid? StationId { get; set; }
    public Guid? ApartmentId { get; set; }
    public bool? IsDefault { get; set; }
    public string? Status { get; set; }
    public StationResponse.StationResponseSimple? Station { get; set; } = null!;
    public ApartmentResponse? Apartment { get; set; } = null!;
    public AreaResponse? Area { get; set; } = null!;
}