using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class BatchsController : BaseController
    {
        private readonly IBatchService _batchService;

        public BatchsController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [SwaggerOperation(Summary = "FarmHub Get All Orders To Process - FARMHUB - {Huu Tri}")]
        [HttpGet("batch/orders-in-businessday/{farmHubId}")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> FarmHubGetAllOrderToProcess(Guid farmHubId)
        {
            var response = await _batchService.FarmHubGetAllOrderToProcess(farmHubId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "CollectedHub Get Batch and All Orders in Batches To Process - COLLECTED_HUB - {Huu Tri}")]
        [HttpGet("batch/{batchId}/orders")]
        //[Authorize(Roles = "CollectedStaff")]
        public async Task<IActionResult> CollectedHubGetAllOrdersInBatch(Guid batchId)
        {
            var response = await _batchService.CollectedHubGetAllOrdersByBatchId(batchId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "FarmHub Get All Batches - FARMHUB - {Huu Tri}")]
        [HttpGet("batches/{farmHubId}")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> FarmHubGetAllBatches(Guid farmHubId)
        {
            var response = await _batchService.FarmHubGetAllBatches(farmHubId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "CollectedHub Get All Batches - COLLECTED_HUB - {Huu Tri}")]
        [HttpGet("batches/collected-hub/{collectedHubId}/business-day/{businessDayId}")]
        //[Authorize(Roles = "CollectedStaff")]
        public async Task<IActionResult> CollectedHubGetAllBatches(Guid collectedHubId, Guid businessDayId)
        {
            var response = await _batchService.CollectedHubGetAllBatches(collectedHubId, businessDayId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "FarmHub Confirm Order for Customer - FARMHUB - {Huu Tri}")]
        [HttpPut("batch/confirmed-order/{orderId}")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> ConfirmedOrder(Guid orderId)
        {
            if (ModelState.IsValid)
            {
                var response = await _batchService.FarmHubConfirmOrderOfCustomer(orderId);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Create Batch - FARMHUB - {Huu Tri}")]
        [HttpPost("batch")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> CreateProductItemForProduct(Guid farmHubId, BatchRequest batchRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _batchService.CreateBatch(farmHubId, batchRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }
    }
}
