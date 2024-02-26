using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Capstone.UniFarm.API.MiddleWares
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var traceId = Guid.NewGuid();
                _logger.LogError($"Error occurred while processing the request, TraceId: {traceId}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problemDetails = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Internal Server Error",
                    Status = (int)StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path,
                    Detail = $"An internal server error occurred, TraceId: {traceId}",
                };

                // Add additional information to problemDetails if necessary
                // For example, you can add the exception message and stack trace
                problemDetails.Extensions.Add("exceptionMessage", ex.Message);
                problemDetails.Extensions.Add("stackTrace", ex.StackTrace);

                // Serialize problemDetails to JSON and write it to the response
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        }

    }
}
