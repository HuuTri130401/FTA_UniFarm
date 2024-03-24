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

        //[SwaggerOperation(Summary = "Create Batch - FARMHUB - {Huu Tri}")]
        //[HttpPost("batch/create")]
        //[Authorize(Roles = "FarmHub")]
        //public async Task<IActionResult> CreateProductItemForProduct(Guid id, ProductItemRequest productItemRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await _batchService.CreateProductItemForProduct(id, productItemRequest);
        //        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        //    }
        //    return BadRequest("Model is invalid");
        //}
    }
}
