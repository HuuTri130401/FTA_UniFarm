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
    public interface IProductItemInMenuService
    {
        Task<OperationResult<bool>> AddProductItemToMenu(Guid menuId, ProductItemInMenuRequest productItemInMenuRequest);
        Task<OperationResult<List<ProductItemInMenuResponse>>> GetProductItemsInMenuByMenuId(Guid menuId);
        Task<OperationResult<bool>> RemoveProductItemFromMenu(Guid productItemInMenuId);
    }
}
