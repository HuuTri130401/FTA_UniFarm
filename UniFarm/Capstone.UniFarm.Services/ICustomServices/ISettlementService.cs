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
    public interface ISettlementService
    {
        Task<OperationResult<bool>> PaymentProfitForFarmHubInBusinessDay(Guid businessDayId, Guid systemAcountId);
        Task<OperationResult<FarmHubSettlementResponse>> GetSettlementForFarmHub(Guid businessDayId, Guid farmHubId);
        Task<OperationResult<FarmHubSettlementResponse>> SystemCreateSettlementForFarmHub();
    }
}
