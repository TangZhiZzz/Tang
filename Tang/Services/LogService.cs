using Microsoft.Extensions.Logging;

namespace Tang.Services
{
    /// <summary>
    /// 日志服务实现
    /// </summary>
    public class LogService : ILogService, ISingleton
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }

        public void Debug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void Info(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void Error(string message, Exception? exception = null, params object[] args)
        {
            if (exception != null)
                _logger.LogError(exception, message, args);
            else
                _logger.LogError(message, args);
        }

        public void Fatal(string message, Exception? exception = null, params object[] args)
        {
            if (exception != null)
                _logger.LogCritical(exception, message, args);
            else
                _logger.LogCritical(message, args);
        }
    }
} 