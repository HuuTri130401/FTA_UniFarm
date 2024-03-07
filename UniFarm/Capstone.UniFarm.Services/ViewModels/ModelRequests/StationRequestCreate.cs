namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record StationRequestCreate
{
    public Guid AreaId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Address { get; set; }
}
