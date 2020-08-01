using Microsoft.Owin.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using Topshelf;
using ElmaTestService.Broadcasting;

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
                try
                {
                    var url = "http://192.168.1.175:5000/signalr";
                    client = new NotificationClient(url);
                }
                catch
                { }
                webServer = WebApp.Start<Startup>(options);
                try
                {
                    var url = "http://localhost:5000/signalr";
                    var client1 = new NotificationClient(url);
                }
                catch
                { }
            }

            public void Stop()
            {
                webServer?.Dispose();
                client?.Dispose();
            }
        }
    }
}
