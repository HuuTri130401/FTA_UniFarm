using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class PriceService : IPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PriceService> _logger;
        private readonly IMapper _mapper;

        public PriceService(IUnitOfWork unitOfWork, ILogger<PriceService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<PriceTableResponse>>> GetAllPrice()
        {
            var result = new OperationResult<List<PriceTableResponse>>();
            try
            {
                var listPrices = await _unitOfWork.PriceTableRepository.GetAllPrice();
                var listPricesResponse = _mapper.Map<List<PriceTableResponse>>(listPrices);

                if (listPricesResponse == null || !listPricesResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"Prices is Empty!", listPricesResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "List Prices Done.", listPricesResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllPrice Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdatePrice(Guid priceTableId, PriceTableRequestUpdate priceTableRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingPriceTable = await _unitOfWork.PriceTableRepository.GetByIdAsync(priceTableId);
                if (existingPriceTable != null)
                {
                    if (priceTableRequestUpdate.Name != null)
                    {
                        existingPriceTable.Name = priceTableRequestUpdate.Name;
                    }
                    if (priceTableRequestUpdate.FromDate != null)
                    {
                        existingPriceTable.FromDate = (DateTime)priceTableRequestUpdate.FromDate;
                    }
                    if (priceTableRequestUpdate.ToDate != null)
                    {
                        existingPriceTable.ToDate = (DateTime)priceTableRequestUpdate.ToDate;
                    }
                    _unitOfWork.PriceTableRepository.Update(existingPriceTable);

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update Price Table have Id: {priceTableId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update Price Table Failed!"); ;
                    }
                } 
                else
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Price Table with Id: {priceTableId}");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UpdatePrice Service Method");
                throw;
            }
        }
    }
}
