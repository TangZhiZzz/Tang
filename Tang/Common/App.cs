using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tang.Services;

namespace Tang.Common
{
    /// <summary>
    /// 应用程序全局静态类
    /// </summary>
    public static class App
    {
        private static IServiceProvider? _serviceProvider;
        private static IConfiguration? _configuration;

        /// <summary>
        /// 初始化应用程序
        /// </summary>
        internal static void Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        public static T GetService<T>() where T : class
        {
            ThrowIfNotInitialized();
            return _serviceProvider!.GetRequiredService<T>();
        }

        /// <summary>
        /// 获取服务（可为空）
        /// </summary>
        public static T? GetServiceOrDefault<T>() where T : class
        {
            ThrowIfNotInitialized();
            return _serviceProvider!.GetService<T>();
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public static T GetConfig<T>(string key) where T : class
        {
            ThrowIfNotInitialized();
            return _configuration!.GetSection(key).Get<T>()
                ?? throw new InvalidOperationException($"Configuration section '{key}' not found.");
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        public static string GetConfigValue(string key)
        {
            ThrowIfNotInitialized();
            return _configuration![key];
        }

        /// <summary>
        /// 获取日志服务
        /// </summary>
        public static ILogService GetLogger()
        {
            return GetService<ILogService>();
        }

        /// <summary>
        /// 获取缓存服务
        /// </summary>
        public static ICacheService GetCache()
        {
            return GetService<ICacheService>();
        }

        /// <summary>
        /// 创建一个作用域
        /// </summary>
        public static IServiceScope CreateScope()
        {
            ThrowIfNotInitialized();
            return _serviceProvider!.CreateScope();
        }

        /// <summary>
        /// 在作用域中执行
        /// </summary>
        public static void ExecuteInScope(Action<IServiceProvider> action)
        {
            using var scope = CreateScope();
            action(scope.ServiceProvider);
        }

        /// <summary>
        /// 在作用域中执行异步操作
        /// </summary>
        public static async Task ExecuteInScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = CreateScope();
            await action(scope.ServiceProvider);
        }

        /// <summary>
        /// 检查是否已初始化
        /// </summary>
        private static void ThrowIfNotInitialized()
        {
            if (_serviceProvider == null || _configuration == null)
            {
                throw new InvalidOperationException("App has not been initialized.");
            }
        }
    }
} 