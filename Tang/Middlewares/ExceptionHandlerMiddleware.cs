using System.Net;
using System.Text.Json;
using Tang.Exceptions;
using Tang.Models;

namespace Tang.Middlewares
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                ApiException apiException => new ApiResult<object>
                {
                    Code = apiException.Code,
                    Message = apiException.Message
                },
                _ => new ApiResult<object>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "服务器内部错误"
                }
            };

            // 记录错误日志
            _logger.LogError(exception, "发生错误: {Message}", exception.Message);

            context.Response.StatusCode = response.Code;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
} 