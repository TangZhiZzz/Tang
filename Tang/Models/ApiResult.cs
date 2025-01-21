namespace Tang.Models
{
    /// <summary>
    /// API统一返回结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public static ApiResult<T> Success(T? data = default, string message = "操作成功")
        {
            return new ApiResult<T>
            {
                Code = 200,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// 失败
        /// </summary>
        public static ApiResult<T> Fail(string message = "操作失败", int code = 400)
        {
            return new ApiResult<T>
            {
                Code = code,
                Message = message
            };
        }
    }
} 