﻿using Microsoft.Owin.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using Topshelf;

namespace ElmaTestService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ServiceApi>(p =>
                {
                    p.ConstructUsing(name => new ServiceApi());
                    p.WhenStarted(tc => tc.Start());
                    p.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription("Elma Host");
                x.SetDisplayName("Elma");
                x.SetServiceName("Elma");

            });
        }
        public class ServiceApi
        {
            IDisposable webServer;
            StartOptions options = new StartOptions();
            
            public ServiceApi()
            {
                options.Urls.Add("http://localhost:5000");
                options.Urls.Add("http://+:5000");
            }
            public void Start()
            {
                webServer = WebApp.Start<Startup>(options);
            }

            public void Stop()
            {
                webServer.Dispose();
            }
        }
    }
}