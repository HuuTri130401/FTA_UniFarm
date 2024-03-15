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
    public interface IMenuService
    {
        Task<OperationResult<List<MenuResponse>>> GetAllMenusByFarmHubAccountId(Guid farmHubAccountId);
        Task<OperationResult<MenuResponse>> GetMenuById(Guid menuId);
        Task<OperationResult<bool>> CreateMenuForFarmHub(Guid farmHubAccountId, MenuRequest menuRequest);
        Task<OperationResult<bool>> AssignMenuToBusinessDay(Guid businessDayId, Guid menuId);
        Task<OperationResult<bool>> DeleteMenu(Guid menuId);
        Task<OperationResult<bool>> UpdateMenu(Guid menuId, MenuRequestUpdate menuRequestUpdate);
    }
}
