using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Repositories.IRepository;

public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
{
    Task<decimal> CalculateCommissionFee(Guid farmHubId, Guid businessDayId);

    Task<OrderDetail> RemoveAsync(OrderDetail entity);
}