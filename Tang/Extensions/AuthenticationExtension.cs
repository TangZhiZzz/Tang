using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tang.Configurations;

namespace Tang.Extensions
{
    public static class AuthenticationExtension
    {
        /// <summary>
        /// 添加认证服务
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册配置
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
            var jwtConfig = configuration.GetSection("JwtConfig").Get<JwtConfig>();

            // 添加认证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig!.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
                    };
                });

            return services;
        }
    }
} 