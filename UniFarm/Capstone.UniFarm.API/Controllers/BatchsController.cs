using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class BatchsController : BaseController
    {
        private readonly IBatchService _batchService;
        private readonly IAccountService _accountService;

        public BatchsController(IBatchService batchService, IAccountService accountService)
        {
            _batchService = batchService;
            _accountService = accountService;
        }

        [SwaggerOperation(Summary = "FarmHub Get All Orders To Process - FARMHUB - {Huu Tri}")]
        [HttpGet("batch/orders-in-businessday/{businessDayId}/farm-hub/{farmHubId}")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> FarmHubGetAllOrderToProcess(Guid businessDayId, Guid farmHubId)
        {
            var response = await _batchService.FarmHubGetAllOrderToProcess(farmHubId, businessDayId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get All Orders Batch - FARMHUB - COLLECTED_HUB - {Huu Tri}")]
        [HttpGet("batch/{batchId}/orders")]
        //[Authorize(Roles = "CollectedStaff, FarmHub")]
        public async Task<IActionResult> GetAllOrdersInBatch(Guid batchId)
        {
            var response = await _batchService.GetAllOrdersInBatch(batchId);
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

        [SwaggerOperation(Summary = "FarmHub Get Batches In Businessday - FARMHUB - {Huu Tri}")]
        [HttpGet("batches/farmhub{farmHubId}/business-day/{businessDayId}")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> FarmHubGetAllBatchesInBussinessDay(Guid farmHubId, Guid businessDayId)
        {
            var response = await _batchService.FarmHubGetAllBatchesInBusinessDay(farmHubId, businessDayId);
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
        public async Task<IActionResult> ConfirmedOrder(Guid orderId, FarHubProcessOrder confirmStatus)
        {
            if (ModelState.IsValid)
            {
                var response = await _batchService.FarmHubConfirmOrderOfCustomer(orderId, confirmStatus);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "CollectedHub Update Received Batch - COLLECTED_HUB - {Huu Tri}")]
        [HttpPut("batch/{batchId}")]
        //[Authorize(Roles = "CollectedHubStaff")]
        public async Task<IActionResult> UpdateBatch(Guid batchId, [FromForm] BatchRequestUpdate batchRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized();
                }
                string token = authHeader.Replace("Bearer ", "");

                var defineUser = _accountService.GetIdAndRoleFromToken(token);
                if (defineUser.Payload == null)
                {
                    return HandleErrorResponse(defineUser!.Errors);
                }
                var collectedStaffId = defineUser.Payload.Id;
                var response = await _batchService.CollectedHubProcessBatch(collectedStaffId, batchId, batchRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "CollectedHub Approved Order for Customer - COLLECTED_HUB - {Huu Tri}")]
        [HttpPut("batch/process-order/{orderId}")]
        //[Authorize(Roles = "CollectedHubStaff")]
        public async Task<IActionResult> ApprovedOrder(Guid orderId, CollectedHubProcessOrder approveStatus)
        {
            if (ModelState.IsValid)
            {
                var response = await _batchService.CollectedHubApprovedOrderOfCustomer(orderId, approveStatus);
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
