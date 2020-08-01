using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using ElmaTestService.Observers.Hub;

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

            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR();
        }
    }
}
