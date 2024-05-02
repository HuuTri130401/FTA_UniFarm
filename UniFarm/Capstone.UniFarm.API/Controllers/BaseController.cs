using Capstone.UniFarm.API.Helpers;
using Capstone.UniFarm.Services.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleErrorResponse(List<Error> errors)
        {
            if (errors.Any(e => e.Code == Services.Commons.StatusCode.UnAuthorize))
            {
                var error = errors.FirstOrDefault(e => e.Code == Services.Commons.StatusCode.UnAuthorize);
                return base.Unauthorized(new ErrorResponse(401, "UnAuthorize", true, error!.Message, DateTime.UtcNow.AddHours(7)));
            }
            if (errors.Any(e => e.Code == Services.Commons.StatusCode.NotFound))
            {
                var error = errors.FirstOrDefault(e => e.Code == Services.Commons.StatusCode.NotFound);
                return base.NotFound(new ErrorResponse(404, "Not Found", true, error!.Message, DateTime.UtcNow.AddHours(7)));
            }
            if (errors.Any(e => e.Code == Services.Commons.StatusCode.ServerError))
            {
                var error = errors.FirstOrDefault(e => e.Code == Services.Commons.StatusCode.ServerError);
                return base.StatusCode(500, new ErrorResponse(500, errors.FirstOrDefault()?.Message == null ? "Server Error" : errors.FirstOrDefault()!.Message, true, error!.Message, DateTime.UtcNow.AddHours(7)));
            }
            return StatusCode(400, new ErrorResponse(400,errors.FirstOrDefault()?.Message == null ? "Bad Request" : errors.FirstOrDefault()!.Message, true, errors, DateTime.UtcNow.AddHours(7)));
        }
    }
}
