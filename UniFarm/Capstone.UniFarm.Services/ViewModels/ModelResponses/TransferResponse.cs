namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record TransferResponse
{
    public Guid Id { get; set; }
    public Guid CollectedId { get; set; }
    public Guid StationId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ExpectedReceiveDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? NoteSend { get; set; }
    public string? NoteReceived { get; set; }
    public string? Code { get; set; }
    public string? Status { get; set; }
    public CollectedHubResponse? Collected { get; set; } = null!;
    public StationResponse.StationResponseSimple? Station { get; set; } = null!;
    public ICollection<OrderResponse.OrderResponseForCustomer>? Orders { get; set; }

    public TransferResponse(Guid id, Guid collectedId, Guid stationId, DateTime? createdAt, DateTime? updatedAt, DateTime? expectedReceiveDate, DateTime? receivedDate, Guid? createdBy, Guid? updatedBy, string? noteSend, string? noteReceived, string? code, string? status, CollectedHubResponse? collected, StationResponse.StationResponseSimple? station, ICollection<OrderResponse.OrderResponseForCustomer>? orders)
    {
        Id = id;
        CollectedId = collectedId;
        StationId = stationId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        ExpectedReceiveDate = expectedReceiveDate;
        ReceivedDate = receivedDate;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        NoteSend = noteSend;
        NoteReceived = noteReceived;
        Code = code;
        Status = status;
        Collected = collected;
        Station = station;
        Orders = orders;
    }
    
}