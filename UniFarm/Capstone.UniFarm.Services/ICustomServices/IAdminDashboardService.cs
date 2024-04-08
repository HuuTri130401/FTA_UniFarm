using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IAdminDashboardService
{
    Task<OperationResult<AdminDashboardResponse.OrderStatisticByBusinessDay>> GetOrderStatisticByBusinessDay(Guid? id, DateTime? businessDay);
}