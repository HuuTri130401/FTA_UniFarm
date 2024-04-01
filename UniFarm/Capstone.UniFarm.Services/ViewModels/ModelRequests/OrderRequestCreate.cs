namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record OrderRequestCreate
{
    public Guid businessDayId { get; set; }
    public Guid stationId { get; set; }
    public Guid apartmentId { get; set; }
    public List<FamHubAndProduct> FamHubAndProduct { get; set; }
    public decimal totalAmount { get; set; }
    public string paymentStatus { get; set; }
    public decimal paymentAmount { get; set; }
    
    public string? shipNote { get; set; }
    
    public OrderRequestCreate(Guid businessDayId, Guid stationId, Guid apartmentId, List<FamHubAndProduct> FamHubAndProduct, decimal TotalAmount, string paymentStatus, decimal paymentAmount, string? shipNote)
    {
        this.businessDayId = businessDayId;
        this.stationId = stationId;
        this.apartmentId = apartmentId;
        this.FamHubAndProduct = FamHubAndProduct;
        this.totalAmount = TotalAmount;
        this.paymentStatus = paymentStatus;
        this.paymentAmount = paymentAmount;
        this.shipNote = shipNote;
    }
    
}

public record FamHubAndProduct
{
    public FamHubAndProduct(Guid farmHubId, List<OrderDetailRequestCreate> orderDetail, decimal totalFarmHubPrice)
    {
        this.farmHubId = farmHubId;
        this.orderDetail = orderDetail;
        this.totalFarmHubPrice = totalFarmHubPrice;
    }
    public Guid farmHubId { get; set; }
    public List<OrderDetailRequestCreate> orderDetail { get; set; }

    public decimal totalFarmHubPrice
    {
        get
        {
            decimal total = 0;
            foreach (var item in orderDetail)
            {
                total += item.TotalPrice;
            }
            return total;
        }
        init { }
    }
}
    
    