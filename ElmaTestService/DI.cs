using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;


namespace ElmaTestService
{
    public class DefaultDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider provider;

        public DefaultDependencyResolver(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public object GetService(Type serviceType)
        {
            return provider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return provider.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }

    public static class IocStartup
    {
        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            // Регистрируем наши хранилища 
            services.AddSingleton<Models.IStoragable<string, string>>(Program.MyStorage);
            services.AddSingleton<IDictionary<string, string>>(Program.OtherServersKeys);

            var controllerTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
              .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
              .Where(t => typeof(ApiController).IsAssignableFrom(t)
                || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase));
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}