namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record AreaResponse
{
    public Guid Id { get; set; }
    public string Province { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Commune { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Code { get; set; } = null!;
}
