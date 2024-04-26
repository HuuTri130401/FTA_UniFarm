using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IPriceService
    {
        Task<OperationResult<List<PriceTableResponse>>> GetAllPrice();
    }
}
