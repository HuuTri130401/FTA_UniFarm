using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record AddToCartRequest
{
    private Guid Id
    {
        get => Guid.NewGuid();
        init { }
    }

    public Guid CustomerId { get; set; }
    
    [Required(ErrorMessage = "FarmHubId is required")]
    public Guid FarmHubId { get; init; }
    public Guid StationId { get; set; }
    public Guid BusinessDayId { get; set; }
    public string? ShipAddress { get; set; }
    [Required(ErrorMessage = "ProductItemId is required")]
    public Guid ProductItemId { get; init; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; init; }
    public bool IsAddToCart { get; set; } = false;
    

    public AddToCartRequest(Guid customerId, Guid farmHubId, Guid productItemId, Guid stationId, Guid businessDayId, string? shipAddress,  int quantity, bool isAddToCart = false)
    {
        Id = this.Id;
        CustomerId = customerId;
        FarmHubId = farmHubId;
        ProductItemId = productItemId;
        StationId = stationId;
        BusinessDayId = businessDayId;
        ShipAddress = shipAddress;
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