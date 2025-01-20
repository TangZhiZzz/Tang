namespace Tang.Models
{
    /// <summary>
    /// API统一响应模型
    /// </summary>
    public class ApiResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; } = string.Empty;

        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public static ApiResult<T> Success(T? data = default) => new() { Code = 200, Msg = "success", Data = data };

        /// <summary>
        /// 失败
        /// </summary>
        public static ApiResult<T> Error(string message) => new() { Code = 500, Msg = message };
    }
} 