using System.Net;
using System.Text.Json;
using TaskBoardApi.Exceptions;
using TaskBoardApi.Model.Errors;

namespace TaskBoardApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
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

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var error = new ApiError();

            switch (ex)
            {
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    error.Message = ex.Message;
                    break;

                case BadRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    error.Message = ex.Message;
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    error.Message = "An unexpected error occurred.";
                    error.Details = _env.IsDevelopment() ? ex.ToString() : null;
                    break;
            }

            error.StatusCode = context.Response.StatusCode;

            return context.Response.WriteAsJsonAsync(error);
        }
    }
}
