using StackExchange.Redis;
using Tang.Configurations;
using Tang.Services;

namespace Tang.Extensions
{
    public static class CacheExtension
    {
        /// <summary>
        /// 添加缓存服务
        /// </summary>
        public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册配置
            services.Configure<CacheConfig>(configuration.GetSection("CacheConfig"));
            var cacheConfig = configuration.GetSection("CacheConfig").Get<CacheConfig>();

            if (cacheConfig!.Type.Equals("Redis", StringComparison.OrdinalIgnoreCase))
            {
                // 注册Redis
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(cacheConfig.RedisConnection));
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            else
            {
                // 注册内存缓存
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }

            return services;
        }
    }
} 