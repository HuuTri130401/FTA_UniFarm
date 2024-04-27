using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class PriceItemService : IPriceItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PriceItemService> _logger;
        private readonly IMapper _mapper;

        public PriceItemService(IUnitOfWork unitOfWork, ILogger<PriceItemService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> UpdatePriceItem(Guid priceItemId, PriceTableItemRequestUpdate priceTableItemRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingPriceItem = await _unitOfWork.PriceTableItemRepository.GetByIdAsync(priceItemId);
                if (existingPriceItem != null)
                {
                    if (priceTableItemRequestUpdate.FromAmount != null)
                    {
                        existingPriceItem.FromAmount = (decimal)priceTableItemRequestUpdate.FromAmount;
                    }
                    if (priceTableItemRequestUpdate.ToAmount != null)
                    {
                        existingPriceItem.ToAmount = (decimal)priceTableItemRequestUpdate.ToAmount;
                    }
                    if (priceTableItemRequestUpdate.Percentage != null)
                    {
                        existingPriceItem.Percentage = (decimal)priceTableItemRequestUpdate.Percentage;
                    }                    
                    if (priceTableItemRequestUpdate.MinFee != null)
                    {
                        existingPriceItem.MinFee = (decimal)priceTableItemRequestUpdate.MinFee;
                    }                    
                    if (priceTableItemRequestUpdate.MaxFee != null)
                    {
                        existingPriceItem.MaxFee = (decimal)priceTableItemRequestUpdate.MaxFee;
                    }
                    var validate = ValidatePriceTableItemUpdate(existingPriceItem, priceTableItemRequestUpdate);
                    if (validate)
                    {
                        _unitOfWork.PriceTableItemRepository.Update(existingPriceItem);
                    }

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update Price Item have Id: {priceItemId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update Price Item Failed!"); 
                    }
                }
                else
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Price Item with Id: {priceItemId}");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UpdatePriceItem Service Method");
                throw;
            }
        }

        private bool ValidatePriceTableItemUpdate(PriceTableItem existingPriceItem, PriceTableItemRequestUpdate priceTableItemRequestUpdate)
        {

            if (priceTableItemRequestUpdate.MinFee != null && priceTableItemRequestUpdate.MaxFee == null)
            {
                if (priceTableItemRequestUpdate.MinFee > existingPriceItem.MaxFee)
                    return false; 

                existingPriceItem.MinFee = (decimal)priceTableItemRequestUpdate.MinFee;
            }

            if (priceTableItemRequestUpdate.MaxFee != null && priceTableItemRequestUpdate.MinFee == null)
            {
                if (priceTableItemRequestUpdate.MaxFee < existingPriceItem.MinFee)
                    return false; 

                existingPriceItem.MaxFee = (decimal)priceTableItemRequestUpdate.MaxFee;
            }

            if (priceTableItemRequestUpdate.MinFee != null && priceTableItemRequestUpdate.MaxFee != null)
            {
                if (priceTableItemRequestUpdate.MinFee < priceTableItemRequestUpdate.MaxFee)
                    return true;
                return false;
            }

            if (priceTableItemRequestUpdate.FromAmount != null && priceTableItemRequestUpdate.ToAmount == null)
            {
                if (priceTableItemRequestUpdate.FromAmount > existingPriceItem.ToAmount)
                    return false; 

                existingPriceItem.FromAmount = (decimal)priceTableItemRequestUpdate.FromAmount;
            }

            if (priceTableItemRequestUpdate.ToAmount != null && priceTableItemRequestUpdate.FromAmount == null)
            {
                if (priceTableItemRequestUpdate.ToAmount < existingPriceItem.FromAmount)
                    return false; 

                existingPriceItem.ToAmount = (decimal)priceTableItemRequestUpdate.ToAmount;
            }

            if (priceTableItemRequestUpdate.FromAmount != null && priceTableItemRequestUpdate.ToAmount != null)
            {
                if (priceTableItemRequestUpdate.FromAmount < priceTableItemRequestUpdate.ToAmount)
                    return true;
                return false;
            }

            return true;
        }

    }
}
