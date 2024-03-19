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
    public interface IFarmHubService
    {
        Task<OperationResult<List<FarmHubResponse>>> GetAllFarmHubs();
        Task<OperationResult<FarmHubResponse>> GetFarmHubById(Guid farmhubId);
        Task<OperationResult<AccountAndFarmHubRequest>> CreateFarmHubShop(AccountAndFarmHubRequest accountAndFarmHubRequest);
        Task<OperationResult<FarmHubResponse>> GetFarmHubInforByFarmHubAccountId(Guid farmHubAccountId);
        Task<OperationResult<bool>> DeleteFarmHub(Guid farmhubId);
        Task<OperationResult<bool>> UpdateFarmHub(Guid farmhubId, FarmHubRequestUpdate farmHubRequestUpdate);
    }
}
