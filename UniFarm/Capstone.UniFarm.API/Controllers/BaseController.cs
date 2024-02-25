using Capstone.UniFarm.API.Helpers;
using Capstone.UniFarm.Services.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class BaseController : ODataController
    {
        protected IActionResult HandleErrorResponse(List<Error> errors)
        {
            if (errors.Any(e => e.Code == ErrorCode.UnAuthorize))
            {
                var error = errors.FirstOrDefault(e => e.Code == ErrorCode.UnAuthorize);
                return Unauthorized(new ErrorResponse(401, "UnAuthorize", error.Message, DateTime.Now));
            }
            if (errors.Any(e => e.Code == ErrorCode.NotFound))
            {
                var error = errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound);
                return NotFound(new ErrorResponse(404, "Not Found", error.Message, DateTime.Now));
            }
            if (errors.Any(e => e.Code == ErrorCode.ServerError))
            {
                var error = errors.FirstOrDefault(e => e.Code == ErrorCode.ServerError);
                return StatusCode(500, new ErrorResponse(500, "Server Error", error.Message, DateTime.Now));
            }
            return StatusCode(400, new ErrorResponse(400, "Bad Request", errors, DateTime.Now));
        }
    }
}
