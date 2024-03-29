using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

namespace Capstone.UniFarm.Repositories.Repository;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public async Task<List<Order>> FarmHubGetAllOrderToProcess(Guid farmhubId, Guid businessDayId)
    {
        return await _dbSet
            .Include(fr => fr.FarmHub)
            .Include(o => o.Customer)
            .Include(o => o.BusinessDay)
            .Include(od => od.OrderDetails)
            .Where(bd => bd.BusinessDayId == businessDayId)
            .Where(fr => fr.FarmHubId == farmhubId)
            .Where(ord => ord.CustomerStatus == "Pending" || ord.CustomerStatus == "Confirmed")
            .Where(ipay => ipay.IsPaid == true)
            .OrderByDescending(st => st.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Order>> FarmHubGetAllOrderToCreateBatch(Guid farmhubId, Guid businessDayId)
    {
        return await _dbSet
            .Where(fr => fr.FarmHubId == farmhubId)
            .Where(bd => bd.BusinessDayId == businessDayId)
            .Where(ord => ord.CustomerStatus == CustomerStatus.Confirmed.ToString())
            .Where(ipay => ipay.IsPaid == true)
            .ToListAsync();
    }

    //public async Task<List<Order>> CollectedHubGetAllOrdersByBatchId(Guid batchId)
    //{
    //    return await _dbSet
    //        .Where(bt => bt.BatchId == batchId)
    //        .OrderBy(sd => sd.ShippedDate)
    //        .ToListAsync();
    //}
}