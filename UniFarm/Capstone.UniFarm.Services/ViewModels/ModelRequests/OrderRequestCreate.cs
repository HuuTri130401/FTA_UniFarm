namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record OrderRequestCreate
{
    public Guid FarmHubId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid StationId { get; set; }
    public Guid BusinessDayId { get; set; }

    // Tạo công thức code
    public decimal? TotalAmount { get; set; }
    public decimal? TotalFarmHubPrice { get; set; }
    public decimal? TotalBenefit { get; set; }

    public string? CustomerStatus { get; set; }
}