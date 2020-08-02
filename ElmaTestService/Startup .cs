using Owin;
using System.Web.Http;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(ElmaTestService.Startup))]

namespace ElmaTestService
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Конфигурируем под self-host.
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{key}",
                defaults: new { key = RouteParameter.Optional }
            );

            // Add IoC
            var serviceProvider = IocStartup.BuildServiceProvider();
            config.DependencyResolver = new DefaultDependencyResolver(serviceProvider);

            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR();
        }
    }
}
