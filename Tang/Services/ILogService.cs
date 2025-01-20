namespace Tang.Services
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// 记录调试日志
        /// </summary>
        void Debug(string message, params object[] args);

        /// <summary>
        /// 记录信息日志
        /// </summary>
        void Info(string message, params object[] args);

        /// <summary>
        /// 记录警告日志
        /// </summary>
        void Warning(string message, params object[] args);

        /// <summary>
        /// 记录错误日志
        /// </summary>
        void Error(string message, Exception? exception = null, params object[] args);

        /// <summary>
        /// 记录致命错误日志
        /// </summary>
        void Fatal(string message, Exception? exception = null, params object[] args);
    }
} 