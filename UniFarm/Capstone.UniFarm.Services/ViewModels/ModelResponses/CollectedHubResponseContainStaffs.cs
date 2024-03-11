namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record CollectedHubResponseContainStaffs
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Address { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Status { get; set; }
    public IEnumerable<AboutMeResponse.StaffResponse>? Staffs { get; set; }
}