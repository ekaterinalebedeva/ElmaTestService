using Microsoft.Owin.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using Topshelf;
using ElmaTestService.Broadcasting;
using ElmaTestService.Observers.Hub;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;

namespace ElmaTestService
{
    class Program
    {            
        /// <summary>
        /// Главное хранилище хэшей
        /// </summary>
        public static Storage<string, string> MyStorage { get; } = new Storage<string, string>();
        /// <summary>
        /// Словарь с ключами, которые находятся на других серверах
        /// </summary>
        public static ConcurrentDictionary<string, string> OtherServersKeys { get; } = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// Коллекция подключений к другим серверам для отправки им уведомлений
        /// </summary>
        public static List<NotificationClient> Clients { get; } = new List<NotificationClient>();
        public static string MyUrl { get; } = "http://192.168.1.201:5000";
        static void Main(string[] args)
        {
            // Присоединяем наблюдателя-сервера, который будет отсылать уведомления клиентам
            MyStorage.Attach(new HubObserver<string>());
            MyStorage.Attach(new ClientObserver<string>());
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
                
                try
                {
                    var url = "http://192.168.1.175:5000";
                    var client = new NotificationClient(url, OtherServersKeys);
                    Clients.Add(client);
                }
                catch
                { }
                webServer = WebApp.Start<Startup>(options);
                //try
                //{
                //    var url = "http://localhost:5000";
                //    var client1 = new NotificationClient(url, Program.OtherServersKeys);
                //    Program.Clients.Add(client1);
                //}
                //catch
                //{ }
            }

            public void Stop()
            {
                webServer?.Dispose();
            }
        }
    }
}
