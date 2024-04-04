using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record AddToCartRequest
{
    private Guid Id
    {
        get => Guid.NewGuid();
        init { }
    }
    
    [Required(ErrorMessage = "FarmHubId is required")]
    public Guid FarmHubId { get; init; }
    public Guid StationId { get; set; }
    public Guid BusinessDayId { get; set; }
    [Required(ErrorMessage = "ProductItemId is required")]
    public Guid ProductItemId { get; init; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public double Quantity { get; init; }
    public bool IsAddToCart { get; set; } = false;
    

    public AddToCartRequest(Guid farmHubId, Guid productItemId, Guid stationId, Guid businessDayId,  double quantity, bool isAddToCart = false)
    {
        Id = this.Id;
        FarmHubId = farmHubId;
        ProductItemId = productItemId;
        StationId = stationId;
        BusinessDayId = businessDayId;
        Quantity = quantity;
        IsAddToCart = isAddToCart;
    }
    
    
    public record CheckExistCartRequest
    {
        public Guid CustomerId { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid ProductItemId { get; set; }
        public Guid StationId { get; set; }
        public Guid BusinessDayId { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}