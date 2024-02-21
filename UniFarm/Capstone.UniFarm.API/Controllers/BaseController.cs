﻿using Capstone.UniFarm.API.Helpers;
using Capstone.UniFarm.Services.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleErrorResponse(List<Error> errors)
        {
            if (errors.Any(e => e.Code == Services.Commons.StatusCode.UnAuthorize))
            {
                var error = errors.FirstOrDefault(e => e.Code == Services.Commons.StatusCode.UnAuthorize);
                return base.Unauthorized(new ErrorResponse(401, "UnAuthorize", error.Message, DateTime.Now));
            }
            if (errors.Any(e => e.Code == Services.Commons.StatusCode.NotFound))
            {
                var error = errors.FirstOrDefault(e => e.Code == Services.Commons.StatusCode.NotFound);
                return base.NotFound(new ErrorResponse(404, "Not Found", error.Message, DateTime.Now));
            }
            if (errors.Any(e => e.Code == Services.Commons.StatusCode.ServerError))
            {
                var error = errors.FirstOrDefault(e => e.Code == Services.Commons.StatusCode.ServerError);
                return base.StatusCode(500, new ErrorResponse(500, "Server Error", error.Message, DateTime.Now));
            }
            return StatusCode(400, new ErrorResponse(400, "Bad Request", errors, DateTime.Now));
        }
    }
}