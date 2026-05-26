using PulsePanel.Core.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace PulsePanel.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    Message = "Internal server error",
                    StatusCode = HttpStatusCode.InternalServerError,
                    TimeStamp = DateTime.UtcNow,
                    Path = context.Request.Path
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}