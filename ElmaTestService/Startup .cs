﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using System.Web.Http;
using System.Web.Mvc;

namespace ElmaTestService
{
    public class Startup
    {
        public static Storage<string> MyStorage { get; } = new Storage<string>();
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{key}",
                defaults: new { key = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);

            MyStorage.Add("1", "fswedfsedfsd");
        }
    }
}