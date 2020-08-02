using Owin;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using System;

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

            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(3);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR();
        }
    }
}
