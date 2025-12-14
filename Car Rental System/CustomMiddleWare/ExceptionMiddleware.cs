using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Car_Rental_System.CustomMiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message = exception.Message;

            switch (exception)
            {
                case ConflictException:
                    status = HttpStatusCode.Conflict;
                    break;
                case UnauthorizedException:
                    status = HttpStatusCode.Unauthorized;
                    break;
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    break;
                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var response = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(response);
        }
    }
}
