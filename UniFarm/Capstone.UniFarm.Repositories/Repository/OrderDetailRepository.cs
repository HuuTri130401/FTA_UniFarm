using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

namespace Capstone.UniFarm.Repositories.Repository;

public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(FTAScript_V1Context context) : base(context)
    {
    }
    

    public new Task DeleteAsync(OrderDetail entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
    
    public new OrderDetail Remove(OrderDetail entity)
    {
        _dbSet.Remove(entity);
        return entity;
    }

    //public async Task<decimal> CalculateCommissionFee(Guid farmHubId, Guid businessDayId)
    //{
    //    // Phí hoa hồng được tính từ SystemPrice trong Category cho mỗi ProductItem
    //    var commissionFees = await _dbSet
    //        .Include(od => od.Order)
    //        .Where(od => od.Order.FarmHubId == farmHubId && od.Order.BusinessDayId == businessDayId)
    //        .Include(pi => pi.ProductItem)
    //        .ThenInclude(pr => pr.Product)
    //        .ThenInclude(c => c.Category)
    //        .SumAsync(od => od.Quantity * od.ProductItem.Product.Category.SystemPrice * (double)od.UnitPrice); 
    //    return (decimal)commissionFees;
    //}
    public async Task<decimal> CalculateCommissionFee(Guid farmHubId, Guid businessDayId)
    {
        // Phí hoa hồng được tính từ SystemPrice trong Category cho mỗi ProductItem
        var commissionFees = await _dbSet
            .Include(od => od.Order)
                .ThenInclude(o => o.FarmHub)
            .Include(od => od.ProductItem)
                .ThenInclude(pi => pi.Product)
                    .ThenInclude(p => p.Category)
            .Where(od => od.Order != null && od.Order.FarmHubId == farmHubId && od.Order.BusinessDayId == businessDayId)
                        .Where(od => od.Order.CustomerStatus == CustomerStatus.CanceledByCollectedHub.ToString()
                          || od.Order.CustomerStatus == CustomerStatus.PickedUp.ToString()
                          || od.Order.CustomerStatus == CustomerStatus.Expired.ToString())
            .SumAsync(od => od.Quantity * (od.ProductItem.Product.Category.SystemPrice / 100) * (double)od.UnitPrice);
        return (decimal)commissionFees;
    }

    public async Task<OrderDetail> RemoveAsync(OrderDetail entity)
    {
        _dbSet.Remove(entity);
        return entity;
    }
}