namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record ProductItemResponseForCustomer
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid FarmHubId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? SpecialTag { get; set; }
    public string? StorageType { get; set; }
    public bool OutOfStock { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Unit { get; set; }
    public string? Status { get; set; }
    public string? ProductOrigin { get; set; }
}