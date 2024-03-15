namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public abstract record OrderResponse
{
    public Guid Id { get; set; }
    public Guid FarmHubId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? StationId { get; set; }
    public Guid? BusinessDayId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Code { get; set; }
    public string? ShipAddress { get; set; }
    public decimal? TotalAmount { get; set; }
    public bool? IsPaid { get; set; }
    
    public record OrderResponseForCustomer : OrderResponse
    {
        public FarmHubResponse? FarmHubResponse { get; set; }
        public BusinessDayResponse? BusinessDayResponse { get; set; }
        public StationResponse? StationResponse { get; set; }
        public OrderDetailResponseForCustomer?  OrderDetailResponseForCustomer { get; set; }
    }
    
    
}