using Capstone.UniFarm.Domain.Models;
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
    public interface IBatchService
    {
        Task<OperationResult<List<OrderResponseToProcess>>> FarmHubGetAllOrderToProcess(Guid farmHubId);
        Task<OperationResult<bool>> FarmHubConfirmOrderOfCustomer(Guid orderId);
        Task<OperationResult<bool>> CreateBatch(Guid farmHubId, BatchRequest batchRequest);
        Task<OperationResult<List<BatchResponse>>> FarmHubGetAllBatches(Guid farmHubId);
        Task<OperationResult<List<BatchResponse>>> CollectedHubGetAllBatches(Guid collectedHubId, Guid businessDayId);
        Task<OperationResult<List<OrderResponseToProcess>>> CollectedHubGetAllOrdersByBatchId(Guid batchId);
    }
}
