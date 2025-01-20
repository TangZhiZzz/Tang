using System.Text.Json;
using Tang.Models;

namespace Tang.Middlewares
{
    public class ResponseHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                var statusCode = context.Response.StatusCode;
                if (statusCode == 200 && !string.IsNullOrEmpty(responseBody))
                {
                    var result = JsonSerializer.Deserialize<object>(responseBody);
                    var wrappedResponse = ApiResult<object>.Success(result);
                    
                    context.Response.ContentType = "application/json";
                    await using var newBodyStream = new MemoryStream();
                    await JsonSerializer.SerializeAsync(newBodyStream, wrappedResponse);
                    var newResponseBody = JsonSerializer.Serialize(wrappedResponse);
                    
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.SetLength(0);
                    await memoryStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(newResponseBody));
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    public static class ResponseHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseHandlerMiddleware>();
        }
    }
} 