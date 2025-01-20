using System.Reflection;
using Tang.Services;

namespace Tang.Extensions
{
    public static class ServiceRegisterExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // 获取所有服务类型
            var serviceTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IServiceRegister).IsAssignableFrom(t));

            foreach (var serviceType in serviceTypes)
            {
                // 获取该类实现的所有接口（除了IServiceRegister和生命周期接口）
                var interfaceTypes = serviceType.GetInterfaces()
                    .Where(i => !IsLifetimeInterface(i));

                // 获取生命周期
                var lifetime = GetServiceLifetime(serviceType);

                // 如果没有实现其他接口，则以其自身类型注册
                if (!interfaceTypes.Any())
                {
                    RegisterService(services, serviceType, serviceType, lifetime);
                    continue;
                }

                // 以所有接口类型注册
                foreach (var interfaceType in interfaceTypes)
                {
                    RegisterService(services, interfaceType, serviceType, lifetime);
                }
            }

            return services;
        }

        /// <summary>
        /// 是否是生命周期接口
        /// </summary>
        private static bool IsLifetimeInterface(Type type)
        {
            return type == typeof(IServiceRegister) ||
                   type == typeof(ITransient) ||
                   type == typeof(IScoped) ||
                   type == typeof(ISingleton);
        }

        /// <summary>
        /// 获取服务生命周期
        /// </summary>
        private static Microsoft.Extensions.DependencyInjection.ServiceLifetime GetServiceLifetime(Type serviceType)
        {
            if (typeof(ITransient).IsAssignableFrom(serviceType))
                return Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient;
            
            if (typeof(ISingleton).IsAssignableFrom(serviceType))
                return Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton;
            
            return Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        private static void RegisterService(
            IServiceCollection services,
            Type serviceType,
            Type implementationType,
            Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
        }
    }
} 