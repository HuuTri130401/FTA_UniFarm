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
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class BatchService : IBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IBatchService> _logger;
        private readonly IMapper _mapper;

        public BatchService(IUnitOfWork unitOfWork, ILogger<IBatchService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<OrderResponseForFarmHub>>> FarmHubGetAllOrderToProcess(Guid farmhubId)
        {
            var result = new OperationResult<List<OrderResponseForFarmHub>>();
            try
            {
                var listOrderProcesss = await _unitOfWork.OrderRepository.FarmHubGetAllOrderToProcess(farmhubId);
                var listOrderProcesssResponse = _mapper.Map<List<OrderResponseForFarmHub>>(listOrderProcesss);

                if (listOrderProcesssResponse == null || !listOrderProcesssResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Orders with FarmHub Id {farmhubId} is Empty!", listOrderProcesssResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Orders Done.", listOrderProcesssResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in FarmHubGetAllOrderToProcess Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> FarmHubConfirmOrderOfCustomer(Guid orderId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingOrder = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (existingOrder == null || existingOrder.CustomerStatus == CustomerStatus.Confirmed.ToString())
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, $"Can not find order with OrderId: : {orderId} or Order has been confirmed!", false);
                    return result;
                }
                existingOrder.CustomerStatus = CustomerStatus.Confirmed.ToString();

                _unitOfWork.OrderRepository.Update(existingOrder);

                var checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    var orderResponseForFarmHub = _mapper.Map<OrderResponseForFarmHub>(existingOrder);
                    result.AddResponseStatusCode(StatusCode.NoContent, $"Confirmed Order have Id: {orderId} Success.", true);
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, $"Confirmed Order have Id: {orderId} Failed!", false);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> CreateBatch(Guid farmHubId, BatchRequest batchRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var listOrderProcesss = await _unitOfWork.OrderRepository.FarmHubGetAllOrderToCreateBatch(farmHubId, batchRequest.BusinessDayId);
                if (listOrderProcesss != null)
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, $"Before Create Batch You Need Confirmed All Order In BusinessDay with Id: {batchRequest.BusinessDayId}", false);
                    return result;
                }

                var CollectedHub = await _unitOfWork.CollectedHubRepository.GetByIdAsync(batchRequest.CollectedId);
                if(CollectedHub == null)
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, $"CollectedHub with Id: {batchRequest.BusinessDayId} not exist!", false);
                    return result;
                }

                var BusinessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(batchRequest.BusinessDayId);
                if (BusinessDay == null)
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, $"CollectedHub with Id: {batchRequest.BusinessDayId} not exist!", false);
                    return result;
                }

                var batch = _mapper.Map<Batch>(batchRequest);
                batch.FarmShipDate = DateTime.Now;
                batch.Status = DeliveryStatus.ShippedToCollectedHub.ToString();

                var checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.Created, "Add Batch Success!", true);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add Batch Failed!"); ;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
