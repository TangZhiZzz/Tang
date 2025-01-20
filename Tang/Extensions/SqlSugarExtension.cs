using SqlSugar;
using StackExchange.Redis;
using Tang.Configurations;
using Tang.Services;

namespace Tang.Extensions
{
    public static class SqlSugarExtension
    {
        /// <summary>
        /// 注册SqlSugar服务
        /// </summary>
        public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册配置
            services.Configure<DbConfig>(configuration.GetSection("DbConfig"));
            var dbConfig = configuration.GetSection("DbConfig").Get<DbConfig>();

            // 注册SqlSugar服务
            services.AddScoped<ISqlSugarClient>(s =>
            {
                return new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = dbConfig!.ConnectionString,
                    DbType = dbConfig.DbType,
                    IsAutoCloseConnection = dbConfig.IsAutoCloseConnection,
                    InitKeyType = InitKeyType.Attribute
                });
            });

            return services;
        }

    }
}