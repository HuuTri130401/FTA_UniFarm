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
        private readonly ICloudinaryService _cloudinaryService;

        public BatchService(IUnitOfWork unitOfWork, ILogger<BatchService> logger, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<OperationResult<List<OrderResponseToProcess>>> FarmHubGetAllOrderToProcess(Guid farmhubId, Guid businessDayId)
        {
            var result = new OperationResult<List<OrderResponseToProcess>>();
            try
            {
                var listOrderProcesss = await _unitOfWork.OrderRepository.FarmHubGetAllOrderToProcess(farmhubId, businessDayId);
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

        public async Task<OperationResult<bool>> FarmHubConfirmOrderOfCustomer(Guid orderId, FarHubProcessOrder farHubProcessOrder)
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

                if (farHubProcessOrder == FarHubProcessOrder.Confirmed)
                {
                    existingOrder.CustomerStatus = farHubProcessOrder.ToString();
                }
                else if (farHubProcessOrder == FarHubProcessOrder.Canceled)
                {
                    existingOrder.CustomerStatus = CustomerStatus.CanceledByFarmHub.ToString();
                    existingOrder.DeliveryStatus = DeliveryStatus.CanceledByFarmHub.ToString();
                }

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

        public async Task<OperationResult<bool>> CollectedHubApprovedOrderOfCustomer(Guid orderId, CollectedHubProcessOrder collectedHubProcessOrder)
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

                if (collectedHubProcessOrder == CollectedHubProcessOrder.Received)
                {
                    existingOrder.CustomerStatus = CustomerStatus.AtCollectedHub.ToString();
                    existingOrder.DeliveryStatus = DeliveryStatus.AtCollectedHub.ToString();
                }
                else if (collectedHubProcessOrder == CollectedHubProcessOrder.Canceled)
                {
                    existingOrder.CustomerStatus = CustomerStatus.CanceledByCollectedHub.ToString();
                    existingOrder.DeliveryStatus = DeliveryStatus.CanceledByCollectedHub.ToString();
                }
                else if(collectedHubProcessOrder == CollectedHubProcessOrder.NotReceived)
                {
                    existingOrder.CustomerStatus = CustomerStatus.CanceledByFarmHub.ToString();
                    existingOrder.DeliveryStatus = DeliveryStatus.CollectedHubNotReceived.ToString();
                }

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
                if (listOrderConfirmed == null || !listOrderConfirmed.Any())
                {
                    result.AddError(StatusCode.BadRequest, $"Can not Create Batch Because not Have Any Order In BusinessDay with Id: {batchRequest.BusinessDayId}");
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
                    order.CustomerStatus = CustomerStatus.OnDelivery.ToString();
                    order.DeliveryStatus = DeliveryStatus.OnTheWayToCollectedHub.ToString();
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

        public async Task<OperationResult<bool>> CollectedHubProcessBatch(Guid collectedStaffId, Guid batchId, BatchRequestUpdate batchRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingBatch = await _unitOfWork.BatchesRepository.GetByIdAsync(batchId);
                if (existingBatch == null)
                {
                    result.AddError(StatusCode.BadRequest, $"Can not find Batch with BatchId: {batchId}!");
                    return result;
                }
                if (batchRequestUpdate.Status == CollectedHubUpdateBatch.Received)
                {
                    existingBatch.Status = CollectedHubUpdateBatch.Received.ToString();
                }
                else if(batchRequestUpdate.Status == CollectedHubUpdateBatch.NotReceived)
                {
                    var listOrdersInBatch = await _unitOfWork.OrderRepository.CollectedHubGetAllOrdersByBatchId(batchId);

                    foreach (var order in listOrdersInBatch)
                    {
                        order.CustomerStatus = CustomerStatus.CanceledByFarmHub.ToString();
                        order.DeliveryStatus = DeliveryStatus.CollectedHubNotReceived.ToString();
                        _unitOfWork.OrderRepository.Update(order);
                    }
                    existingBatch.Status = CollectedHubUpdateBatch.NotReceived.ToString();
                }

                var feedBackImage = _cloudinaryService.UploadImageAsync(batchRequestUpdate.FeedBackImage);
                existingBatch.FeedBackImage = await feedBackImage;
                existingBatch.ReceivedDescription = batchRequestUpdate.ReceivedDescription;
                existingBatch.CollectedHubReceiveDate = DateTime.UtcNow.AddHours(7);
                existingBatch.CollectedStaffProcessId = collectedStaffId;

                _unitOfWork.BatchesRepository.Update(existingBatch);

                var checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.NoContent, $"Confirmed Batch have Id: {batchId} Success.", true);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, $"Confirmed Batch have Id: {batchId} Failed!");
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
