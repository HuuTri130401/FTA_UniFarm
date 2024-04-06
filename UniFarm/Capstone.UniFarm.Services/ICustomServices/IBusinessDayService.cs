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
        Task<OperationResult<BusinessDayResponse>> GetBusinessDayById(Guid businessDayId);
        Task<OperationResult<bool>> CreateBusinessDay(BusinessDayRequest businessDayRequest);

        //bool IsValidBusinessDay(BusinessDayRequest businessDayRequest);
        Task<OperationResult<bool>> DeleteBusinessDay(Guid businessDayId);
        Task<OperationResult<bool>> UpdateBusinessDay(Guid businessDayId, BusinessDayRequestUpdate businessDayRequestUpdate);
        Task UpdateEndOfDayForAllBusinessDays();
    }
}
