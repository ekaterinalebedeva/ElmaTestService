using Microsoft.Owin.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using Topshelf;
using ElmaTestService.Broadcast;

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
            IDisposable client;
            public ServiceApi()
            {
                options.Urls.Add("http://localhost:5000");
                options.Urls.Add("http://+:5000");
            }
            public void Start()
            {
                client = new NotificationClient("http://192.168.1.175:5000");
                webServer = WebApp.Start<Startup>(options);
            }

            public void Stop()
            {
                webServer.Dispose();
                client.Dispose();
            }
        }
    }
}
