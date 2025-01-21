namespace Tang.Exceptions
{
    /// <summary>
    /// API异常
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// 创建API异常
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="code">状态码</param>
        public ApiException(string message, int code = 400) : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// 创建API异常
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        /// <param name="code">状态码</param>
        public ApiException(string message, Exception innerException, int code = 400) 
            : base(message, innerException)
        {
            Code = code;
        }
    }
} 