using Microsoft.OpenApi.Models;
using System.Reflection;
using Tang.Extensions;
using Tang.Middlewares;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tang.Services;
using Tang.Configurations;
using StackExchange.Redis;
using Serilog;
using Serilog.Events;

namespace Tang
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Tang API", 
                    Version = "v1",
                    Description = "Tang API 接口文档",
                    Contact = new OpenApiContact
                    {
                        Name = "Tang",
                        Email = "tang@example.com"
                    }
                });

                // 添加XML注释文档
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // 添加对控制器的注释
                c.CustomSchemaIds(type => type.FullName);

                // 添加JWT认证配置
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // 注册SqlSugar服务
            builder.Services.AddSqlSugar(builder.Configuration);

            // 注册认证服务
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // 注册缓存服务
            builder.Services.AddCacheService(builder.Configuration);

            // 注册其他服务
            builder.Services.AddServices();

            // 配置日志服务
            builder.Host.AddSerilogService(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            // 初始化数据库
            app.UseInitDatabase();

            // 注册响应处理中间件
            app.UseResponseHandler();

            app.MapControllers();

            app.Run();
        }
    }
}
