using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Repositories.IRepository;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<Order>> FarmHubGetAllOrderToProcess(Guid farmhubId, Guid businessDayId);
    Task<List<Order>> FarmHubGetAllOrderToCreateBatch(Guid farmhubId, Guid businessDayId);
    Task<List<Order>> CollectedHubGetAllOrdersByBatchId(Guid batchId);
    Task<bool> CheckAllOrderProcessedByCollectedHub(Guid batchId);
    Task<bool> AreAllOrdersCompletedForBusinessDay(Guid businessDayId);
    Task<List<Order>> GetAllOrderToUpdateEndOfDay(Guid businessDayId);
    //Task<List<FarmHubSettlement>> SystemCalculateTotalForFarmHub(Guid businessDayId);
    //Task<Dictionary<Guid, decimal?>> CalculateTotalForBusinessDayByFarmHub(Guid businessDayId);
    Task<Dictionary<Guid, (decimal? TotalAmount, int OrderCount)>> CalculateTotalForBusinessDayByFarmHub(Guid businessDayId);
    Task<(decimal? TotalAmount, int OrderCount)> CalculateTotalForBusinessDayOfOneFarmHub(Guid businessDayId, Guid farmHubId);
}