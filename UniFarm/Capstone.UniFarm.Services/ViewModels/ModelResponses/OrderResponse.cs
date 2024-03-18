namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public abstract record OrderResponse
{
    OrderResponse( Guid id, Guid farmHubId, Guid customerId, Guid? stationId, Guid? businessDayId, DateTime? createdAt, string? code, string? shipAddress, decimal? totalAmount, bool? isPaid)
    {
        Id = id;
        FarmHubId = farmHubId;
        CustomerId = customerId;
        StationId = stationId;
        BusinessDayId = businessDayId;
        CreatedAt = createdAt;
        Code = code;
        ShipAddress = shipAddress;
        TotalAmount = totalAmount;
        IsPaid = isPaid;
    }

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
        public OrderResponseForCustomer(
            Guid id, 
            Guid farmHubId, 
            Guid customerId, 
            Guid? stationId, 
            Guid? businessDayId, 
            DateTime? createdAt, 
            string? code, 
            string? shipAddress, 
            decimal? totalAmount, 
            bool? isPaid,
            FarmHubResponse? farmHubResponse,
            BusinessDayResponse? businessDayResponse,
            StationResponse? stationResponse,
            OrderDetailResponseForCustomer? orderDetailResponse
            ) : base(id, farmHubId, customerId, stationId, businessDayId, createdAt, code, shipAddress, totalAmount, isPaid)
        {
            FarmHubResponse = farmHubResponse;
            BusinessDayResponse = businessDayResponse;
            StationResponse = stationResponse;
            OrderDetailResponse = orderDetailResponse;
        }
        
        public FarmHubResponse? FarmHubResponse { get; set; }
        public BusinessDayResponse? BusinessDayResponse { get; set; }
        public StationResponse? StationResponse { get; set; }
        public OrderDetailResponseForCustomer?  OrderDetailResponse { get; set; }
    }
    
}