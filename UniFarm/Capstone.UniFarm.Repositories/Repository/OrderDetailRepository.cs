using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Repositories.Repository;

public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(FTAScript_V1Context context) : base(context)
    {
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
            .SumAsync(od => od.Quantity * od.ProductItem.Product.Category.SystemPrice * (double)od.UnitPrice);
        return (decimal)commissionFees;
    }

}