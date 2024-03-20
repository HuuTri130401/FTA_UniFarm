using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record TransferRequestCreate
{
    private Guid Id
    {
        get => Guid.NewGuid();
        init { }
    }

    [Required(ErrorMessage = "CollectedId is required")]
    public Guid CollectedId { get; set; }

    [Required(ErrorMessage = "StationId is required")]
    public Guid StationId { get; set; }

    public string? NoteSend { get; set; }
    
    public DateTime? ExpectedReceiveDate { get; set; }
    
    [Required(ErrorMessage = "OrderIds is required")]
    public Guid[] OrderIds { get; set; }


    public TransferRequestCreate(Guid collectedId, Guid stationId, string? noteSend, DateTime? expectedReceiveDate, Guid[] orderIds)
    {
        Id = this.Id;
        CollectedId = collectedId;
        StationId = stationId;
        NoteSend = noteSend;
        ExpectedReceiveDate = expectedReceiveDate;
        OrderIds = orderIds;
    }
}