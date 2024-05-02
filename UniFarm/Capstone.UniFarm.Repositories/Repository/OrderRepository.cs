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
            .Include(st => st.Station)
            .Include(fr => fr.FarmHub)
            .Include(o => o.Customer)
            .Include(o => o.BusinessDay)
            .Include(od => od.OrderDetails)
            .ThenInclude(odi => odi.ProductItem)
            .Where(bd => bd.BusinessDayId == businessDayId)
            .Where(fr => fr.FarmHubId == farmhubId)
            //.Where(ord => ord.CustomerStatus == "Pending" || ord.CustomerStatus == "Confirmed")
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

    public async Task<List<Order>> CollectedHubGetAllOrdersByBatchId(Guid batchId)
    {
        return await _dbSet
            .Where(bt => bt.BatchId == batchId)
            .ToListAsync();
    }

    public async Task<bool> CheckAllOrderProcessedByCollectedHub(Guid batchId)
    {
        var listOrders = await _dbSet
                        .Where(bt => bt.BatchId == batchId)
                        .Where(de => de.DeliveryStatus == DeliveryStatus.Pending.ToString())
                        .ToListAsync();
        if (listOrders == null || !listOrders.Any())
        {
            return true;
        }
        return false;
    }

    public async Task<List<Order>> GetAllOrderToUpdateEndOfDay(Guid businessDayId)
    {
        return await _dbSet
            .Where(bd => bd.BusinessDayId == businessDayId)
            .Where(ord => ord.CustomerStatus == CustomerStatus.Confirmed.ToString())
            .Where(ipay => ipay.IsPaid == true)
            .ToListAsync();
    }

    private readonly HashSet<OrderStatusCompleted> _completedStatuses = new HashSet<OrderStatusCompleted>
    {
        OrderStatusCompleted.CanceledByCustomer,
        OrderStatusCompleted.CanceledByFarmHub,
        OrderStatusCompleted.CanceledByCollectedHub,
        OrderStatusCompleted.Expired,
        OrderStatusCompleted.PickedUp
    };

    public async Task<bool> AreAllOrdersCompletedForBusinessDay(Guid businessDayId)
    {
        var listOrdersCompleted = await _dbSet
            .Where(bd => bd.BusinessDayId == businessDayId)
            .ToListAsync();

        if (listOrdersCompleted == null || !listOrdersCompleted.Any())
        {
            return false;
        }
        //eturn listOrdersCompleted.All(order => _completedStatuses.Contains(Enum.Parse<OrderStatusCompleted>(order.CustomerStatus)));
        return listOrdersCompleted.All(order =>
            Enum.TryParse<OrderStatusCompleted>(order.CustomerStatus, out var status) &&
            _completedStatuses.Contains(status));
    }

    //public async Task<List<FarmHubSettlement>> SystemCalculateTotalForFarmHub(Guid businessDayId)
    //{
    //    var totals = await _dbSet
    //        .Where(bt => bt.BusinessDayId == businessDayId)
    //        .Where(de => de.DeliveryStatus == DeliveryStatus.CanceledByCollectedHub.ToString()
    //            || de.DeliveryStatus == CustomerStatus.PickedUp.ToString()
    //            || de.DeliveryStatus == CustomerStatus.Expired.ToString())
    //        .GroupBy(f => f.FarmHubId)
    //        .Select(group => new FarmHubSettlement
    //        {
    //            BusinessDayId = businessDayId,
    //            FarmHubId = group.Key,
    //            TotalSales = (decimal)group.Sum(o => o.TotalAmount)
    //        })
    //        .ToListAsync();
    //    return totals;
    //}

    //public async Task<Dictionary<Guid, decimal?>> CalculateTotalForBusinessDayByFarmHub(Guid businessDayId)
    //{
    //    // Group orders by FarmHubId and calculate the total for each group
    //    var totals = await _dbSet
    //        .Where(o => o.BusinessDayId == businessDayId)
    //                .Where(cus => cus.CustomerStatus == CustomerStatus.CanceledByCollectedHub.ToString()
    //        || cus.CustomerStatus == CustomerStatus.PickedUp.ToString()
    //        || cus.CustomerStatus == CustomerStatus.Expired.ToString())
    //        .GroupBy(o => o.FarmHubId)
    //        .Select(g => new { FarmHubId = g.Key, TotalAmount = g.Sum(o => o.TotalAmount) })
    //        .ToDictionaryAsync(g => g.FarmHubId, g => g.TotalAmount);
    //    return totals;
    //}

    public async Task<Dictionary<Guid, (decimal? TotalAmount, int OrderCount)>> CalculateTotalForBusinessDayByFarmHub(Guid businessDayId)
    {
        // Group orders by FarmHubId and calculate the total amount and count for each group
        var totalAmountAndNumOrder = await _dbSet
            .Where(o => o.BusinessDayId == businessDayId)
            .Where(cus => cus.CustomerStatus == CustomerStatus.CanceledByCollectedHub.ToString()
                          || cus.CustomerStatus == CustomerStatus.PickedUp.ToString()
                          || cus.CustomerStatus == CustomerStatus.Expired.ToString())
            .GroupBy(o => o.FarmHubId)
            .Select(g => new
            {
                FarmHubId = g.Key,
                TotalAmount = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .ToDictionaryAsync(g => g.FarmHubId, g => (g.TotalAmount, g.OrderCount));

        return totalAmountAndNumOrder;
    }

    public async Task<(decimal? TotalAmount, int OrderCount)> CalculateTotalForBusinessDayOfOneFarmHub(Guid businessDayId, Guid farmHubId)
    {
        // Lấy tổng số lượng và tổng số tiền của các đơn hàng từ một FarmHub cụ thể trong một BusinessDay
        //Tổng tiền của farmhub này bao gồm đơn hàng đã hoàn thành: Khi khách đã nhận, khi khách quá hạn nhận, khi bị hủy bỏ bởi hệ thống
        var result = await _dbSet
            .Where(o => o.BusinessDayId == businessDayId && o.FarmHubId == farmHubId)
            .Where(cus => cus.CustomerStatus == CustomerStatus.CanceledByCollectedHub.ToString()
                          || cus.CustomerStatus == CustomerStatus.PickedUp.ToString()
                          || cus.CustomerStatus == CustomerStatus.Expired.ToString())
            .GroupBy(o => o.FarmHubId) 
            .Select(g => new
            {
                TotalAmount = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .FirstOrDefaultAsync(); 

        return (result?.TotalAmount, result?.OrderCount ?? 0);
    }

    public async Task<bool> IsOrderCancelledAsync(Order order)
    {
        return await Task.FromResult(IsOrderCancelled(order));
    }

    public Task DeleteAsync(Order order)
    {
        _dbSet.Remove(order);
        return Task.CompletedTask;
    }

    private bool IsOrderCancelled(Order order)
    {
        return order.CustomerStatus == CustomerStatus.CanceledByCustomer.ToString() ||
               order.CustomerStatus == CustomerStatus.CanceledByFarmHub.ToString() ||
               order.CustomerStatus == CustomerStatus.CanceledByCollectedHub.ToString();
    }
}