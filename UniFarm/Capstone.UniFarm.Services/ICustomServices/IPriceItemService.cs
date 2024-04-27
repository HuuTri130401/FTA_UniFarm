using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IPriceItemService
    {
        Task<OperationResult<bool>> UpdatePriceItem(Guid priceItemId, PriceTableItemRequestUpdate priceTableItemRequestUpdate);
    }
}
