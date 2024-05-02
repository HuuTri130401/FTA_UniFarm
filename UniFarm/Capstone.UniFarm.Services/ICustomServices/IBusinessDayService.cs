using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IBusinessDayService
    {
        Task<OperationResult<List<BusinessDayResponse>>> GetAllBusinessDays();
        Task<OperationResult<List<BusinessDayContainBatchStatistics>>> GetAllBusinessDaysContainBatchQuantity(Guid collectedHubId);
        Task<OperationResult<BusinessDayResponse>> GetBusinessDayById(Guid businessDayId);
        Task<OperationResult<BusinessDayResponse>> FarmHubGetBusinessDayById(Guid farmHubAccountId, Guid businessDayId);
        Task<OperationResult<bool>> CreateBusinessDay(BusinessDayRequest businessDayRequest);
        Task<OperationResult<bool>> DeleteBusinessDay(Guid businessDayId);
        Task<OperationResult<bool>> StopSellingDay(Guid businessDayId);
        Task<OperationResult<bool>> UpdateBusinessDay(Guid businessDayId, BusinessDayRequestUpdate businessDayRequestUpdate);
        Task UpdateEndOfDayForAllBusinessDays();
        Task CheckAndStopSellingDayJob();
        Task<OperationResult<bool>> RemoveProductItemInCartJob();
    }
}
