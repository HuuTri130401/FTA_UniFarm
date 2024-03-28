using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Repositories.IRepository;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<Order>> FarmHubGetAllOrderToProcess(Guid farmhubId);
    Task<List<Order>> FarmHubGetAllOrderToCreateBatch(Guid farmhubId, Guid businessDayId);
    //Task<List<Order>> CollectedHubGetAllOrdersByBatchId(Guid batchId);
}