using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;
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
        private readonly ILogger<BatchService> _logger;
        private readonly IMapper _mapper;

        public BatchService(IUnitOfWork unitOfWork, ILogger<BatchService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<OrderResponseToProcess>>> FarmHubGetAllOrderToProcess(Guid farmhubId)
        {
            var result = new OperationResult<List<OrderResponseToProcess>>();
            try
            {
                var listOrderProcesss = await _unitOfWork.OrderRepository.FarmHubGetAllOrderToProcess(farmhubId);
                var listOrderProcesssResponse = _mapper.Map<List<OrderResponseToProcess>>(listOrderProcesss);

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

        public async Task<OperationResult<List<BatchDetailResponse>>> GetAllOrdersInBatch(Guid batchId)
        {
            var result = new OperationResult<List<BatchDetailResponse>>();
            try
            {
                var listOrdersInBatch = await _unitOfWork.BatchesRepository.GetAllOrdersInBatch(batchId);
                var listOrdersInBatchResponse = _mapper.Map<List<BatchDetailResponse>>(listOrdersInBatch);

                if (listOrdersInBatchResponse == null || !listOrdersInBatchResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Orders with Batch Id {batchId} is Empty!", listOrdersInBatchResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Orders Done.", listOrdersInBatchResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllOrdersInBatch Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> FarmHubConfirmOrderOfCustomer(Guid orderId, ConfirmStatus confirmStatus)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingOrder = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (existingOrder == null)
                {
                    result.AddError(StatusCode.BadRequest, $"Can not find order with OrderId: {orderId}!");
                    return result;
                }
                existingOrder.CustomerStatus = confirmStatus.ToString();

                _unitOfWork.OrderRepository.Update(existingOrder);

                var checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.NoContent, $"Confirmed Order have Id: {orderId} Success.", true);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, $"Confirmed Order have Id: {orderId} Failed!");
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> CollectedHubApprovedOrderOfCustomer(Guid orderId, ApproveStatus approveStatus)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingOrder = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (existingOrder == null)
                {
                    result.AddError(StatusCode.BadRequest, $"Can not find order with OrderId: {orderId}!");
                    return result;
                }
                existingOrder.CustomerStatus = approveStatus.ToString();

                _unitOfWork.OrderRepository.Update(existingOrder);

                var checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.NoContent, $"Confirmed Order have Id: {orderId} Success.", true);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, $"Confirmed Order have Id: {orderId} Failed!");
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
                var CollectedHub = await _unitOfWork.CollectedHubRepository.GetByIdAsync(batchRequest.CollectedId);
                if (CollectedHub == null)
                {
                    result.AddError(StatusCode.BadRequest, $"CollectedHub with Id: {batchRequest.CollectedId} not exist!");
                    return result;
                }

                var BusinessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(batchRequest.BusinessDayId);
                if (BusinessDay == null)
                {
                    result.AddError(StatusCode.BadRequest, $"BusinessDay with Id: {batchRequest.BusinessDayId} not exist!");
                    return result;
                }

                var listOrderConfirmed = await _unitOfWork.OrderRepository.FarmHubGetAllOrderToCreateBatch(farmHubId, batchRequest.BusinessDayId);
                if (listOrderConfirmed == null)
                {
                    result.AddError(StatusCode.BadRequest, $"Before Create Batch You Need Confirmed Order In BusinessDay with Id: {batchRequest.BusinessDayId}");
                    return result;
                }

                //var checkListOrder = await _unitOfWork.OrderRepository.FarmHubGetAllOrderToProcess(farmHubId);
                //if(checkListOrder == null)
                //{
                //    result.AddError(StatusCode.BadRequest, $"Can not Create Batch Because not Have Any Order!");
                //    return result;
                //}

                var batch = _mapper.Map<Batch>(batchRequest);
                batch.Id = Guid.NewGuid();
                batch.FarmShipDate = DateTime.UtcNow.AddHours(7);
                batch.FarmHubId = farmHubId;
                batch.Status = BatchStatus.Pending.ToString();

                await _unitOfWork.BatchesRepository.AddAsync(batch);

                foreach (var order in listOrderConfirmed)
                {
                    order.BatchId = batch.Id; // Update Batch For Each Order
                    order.CollectedHubId = CollectedHub.Id; // Update CollecedId For FarmHub Ship 
                    order.CustomerStatus = CustomerStatus.OnTheWayToCollectedHub.ToString();
                    order.DeliveryStatus = DeliveryStatus.OnTheWayToCollectedHub.ToString() + $" {CollectedHub.Name}";
                    _unitOfWork.OrderRepository.Update(order);
                }

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

        public async Task<OperationResult<List<BatchResponse>>> FarmHubGetAllBatches(Guid farmHubId)
        {
            var result = new OperationResult<List<BatchResponse>>();
            try
            {
                var listBatches = await _unitOfWork.BatchesRepository.GetAllBatchesByFarmHubId(farmHubId);
                var listBatchesResponse = _mapper.Map<List<BatchResponse>>(listBatches);
                if(listBatchesResponse == null || !listBatchesResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Batches with FarmHubId: {farmHubId} is Empty!", listBatchesResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Batches Done.", listBatchesResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in FarmHubGetAllBatch Service!");
                throw;
            }
        }

        public async Task<OperationResult<List<BatchResponse>>> CollectedHubGetAllBatches(Guid collectedHubId, Guid businessDayId)
        {
            var result = new OperationResult<List<BatchResponse>>();
            try
            {
                var listBatches = await _unitOfWork.BatchesRepository.GetAllBatchesInBusinessDay(collectedHubId, businessDayId);
                var listBatchesResponse = _mapper.Map<List<BatchResponse>>(listBatches);
                if (listBatchesResponse == null || !listBatchesResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Batches with BusinessDayId: {businessDayId} is Empty!", listBatchesResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Batches Done.", listBatchesResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CollectedHubGetAllBatches Service!");
                throw;
            }
        }
    }
}
