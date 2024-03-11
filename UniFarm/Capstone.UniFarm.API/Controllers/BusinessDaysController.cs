using Capstone.UniFarm.Repositories.IRepository;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class BusinessDaysController : BaseController
    {
        private readonly IBusinessDayService _businessDayService;

        public BusinessDaysController(IBusinessDayService businessDayService)
        {
            _businessDayService = businessDayService;
        }

        [SwaggerOperation(Summary = "Get All BusinessDays - Admin, FarmHub Role - {Huu Tri}")]
        [HttpGet("business-days")]
        public async Task<IActionResult> GetAllBusinessDays()
        {
            var response = await _businessDayService.GetAllBusinessDays();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Business Day By Id - Admin Role & FarmHub Role - {Huu Tri}")]
        [HttpGet("business-day/{id}")]
        public async Task<IActionResult> GetBusinessDayById(Guid id)
        {
            var response = await _businessDayService.GetBusinessDayById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Business Day - Admin Role - {Huu Tri}")]
        [HttpPost("business-day")]
        public async Task<IActionResult> CreateBusinessDay(BusinessDayRequest businessDayRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _businessDayService.CreateBusinessDay(businessDayRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "InActive BusinessDay - Admin Role - {Huu Tri}")]
        [HttpDelete("business-day/{id}")]
        public async Task<IActionResult> DeleteBusinessDay(Guid id)
        {
            var response = await _businessDayService.DeleteBusinessDay(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
