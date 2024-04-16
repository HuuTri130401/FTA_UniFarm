namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public abstract record OrderResponse
{
    OrderResponse(Guid id, Guid farmHubId, Guid customerId, Guid? stationId, Guid? businessDayId, DateTime? createdAt,
        string? code, string? shipAddress, decimal? totalAmount, bool? isPaid)
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

    public record OrderResponseForCustomer
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
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? CustomerStatus { get; set; }
        public FarmHubResponse? FarmHubResponse { get; set; }
        public BusinessDayResponse? BusinessDayResponse { get; set; }
        public StationResponse.StationResponseSimple? StationResponse { get; set; }
        public List<OrderDetailResponseForCustomer>? OrderDetailResponse { get; set; }
    }
    
    public record OrderResponseForStaff
    {
        public Guid Id { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? StationId { get; set; }
        public Guid? BusinessDayId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpectedReceiveDate { get; set; }
        public string? Code { get; set; }
        public string? ShipAddress { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? CustomerStatus { get; set; }
        public FarmHubResponse? FarmHubResponse { get; set; }
        public BusinessDayResponse? BusinessDayResponse { get; set; }
        public StationResponse.StationResponseSimple? StationResponse { get; set; }
        public List<OrderDetailResponse>? OrderDetailResponse { get; set; }
        public BatchResponseSimple? BatchResponse { get; set; }
        public TransferResponse.TransferResponseSimple? TransferResponse { get; set; }
        public AboutMeResponse.CustomerResponseSimple? CustomerResponse { get; set; }
    }

    public record OrderAndOrderDetailResponse
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpectedReceiveDate { get; set; }
        public string? Code { get; set; }
        public string? ShipAddress { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? CustomerStatus { get; set; }
        public FarmHubResponse? FarmHubResponse { get; set; }
        public AboutMeResponse.CustomerResponseSimple? CustomerResponse { get; set; }
        public BusinessDayResponse? BusinessDayResponse { get; set; }
        public StationResponse.StationResponseSimple? StationResponse { get; set; }
        public BatchResponseSimple? BatchResponse { get; set; }
        public CollectedHubResponse? CollectedHubResponse { get; set; }
        public TransferResponse.TransferResponseSimple? TransferResponse { get; set; }
        public List<OrderDetailResponse>? OrderDetailResponse { get; set; }
    }
}