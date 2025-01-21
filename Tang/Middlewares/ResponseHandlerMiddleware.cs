using System.Text.Json;
using Tang.Models;

namespace Tang.Middlewares
{
    public class ResponseHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonOptions;

        public ResponseHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
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
                    // 1. 检查是否是ApiResult
                    if (IsApiResult(responseBody))
                    {
                        await WriteToResponseBody(memoryStream, responseBody, originalBodyStream);
                        return;
                    }

                    // 2. 检查是否是JSON格式
                    if (IsValidJson(responseBody))
                    {
                        var result = JsonSerializer.Deserialize<object>(responseBody, _jsonOptions);
                        await WrapAndWriteResponse(memoryStream, result, statusCode, originalBodyStream);
                        return;
                    }

                    // 3. 作为普通字符串处理
                    await WrapAndWriteResponse(memoryStream, responseBody, statusCode, originalBodyStream);
                }
                else
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalBodyStream);
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private bool IsApiResult(string json)
        {
            try
            {
                var result = JsonSerializer.Deserialize<ApiResult<object>>(json, _jsonOptions);
                // 检查必要的属性是否存在且符合预期
                return result != null 
                    && result.Code >= 0  // 确保有状态码
                    && !string.IsNullOrEmpty(result.Message);  // 确保有消息
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidJson(string strInput)
        {
            try
            {
                JsonSerializer.Deserialize<object>(strInput, _jsonOptions);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task WrapAndWriteResponse(MemoryStream memoryStream, object? result, int statusCode, Stream originalBodyStream)
        {
            var wrapper = new ApiResult<object>
            {
                Code = statusCode,
                Message = "Success",
                Data = result
            };

            var responseBody = JsonSerializer.Serialize(wrapper, _jsonOptions);
            await WriteToResponseBody(memoryStream, responseBody, originalBodyStream);
        }

        private static async Task WriteToResponseBody(MemoryStream memoryStream, string content, Stream originalBodyStream)
        {
            memoryStream.SetLength(0);
            using var writer = new StreamWriter(memoryStream, leaveOpen: true);
            await writer.WriteAsync(content);
            await writer.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
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