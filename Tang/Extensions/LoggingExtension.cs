using Serilog;
using Serilog.Events;
using Tang.Configurations;

namespace Tang.Extensions
{
    public static class LoggingExtension
    {
        /// <summary>
        /// 添加日志服务
        /// </summary>
        public static IHostBuilder AddSerilogService(this IHostBuilder builder, IConfiguration configuration)
        {
            // 注册配置
            var logConfig = configuration.GetSection("LogConfig").Get<LogConfig>();

            // 配置Serilog
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    logConfig!.FilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: logConfig.RetainedDays);

            if (logConfig.WriteToConsole)
            {
                loggerConfiguration.WriteTo.Console();
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            return builder.UseSerilog();
        }
    }
} 