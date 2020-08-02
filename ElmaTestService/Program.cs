using Microsoft.Owin.Hosting;
using System;
using Topshelf;
using ElmaTestService.Broadcasting;
using ElmaTestService.Models;
using ElmaTestService.Observers.Hub;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Microsoft.Owin.BuilderProperties;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;

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
        public static BlockingCollection<NotificationClient> Clients { get; } = new BlockingCollection<NotificationClient>();
        /// <summary>
        /// Порт. Задается в app.config
        /// </summary>
        public static string Port { get; set; } = ConfigurationManager.AppSettings.Get("Port");
        /// <summary>
        /// Свой IP в локальной сети
        /// </summary>
        public static string MyIP { get; set; }
        /// <summary>
        /// Полный URL в локальной сети
        /// </summary>
        public static string MyUrl { get; set; }
        static void Main(string[] args)
        {
            SetUrl();

            // Присоединяем наблюдателя-сервера, который будет отсылать уведомления клиентам.
            MyStorage.Attach(new HubObserver<string>(MyStorage, OtherServersKeys));
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
        /// <summary>
        /// Пытаемся получить свой IP в локальной сети
        /// </summary>
        static void SetUrl()
        {
            string hostName = Dns.GetHostName(); 
            Console.WriteLine($"hostName {hostName}");
            MyIP = Dns.GetHostEntry(hostName).AddressList.LastOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            if (string.IsNullOrEmpty(MyIP))
            {
                throw new Exception("Не удалось получить адрес в локальной сети");
            }
            MyUrl = $"http://{MyIP}:{Port}";
            Console.WriteLine($"IP {MyUrl}");
        }
        public class ServiceApi
        {
            private IDisposable _webServer;
            private StartOptions _options = new StartOptions();
            public ServiceApi()
            {
                _options.Urls.Add(MyUrl);
            }
            public void Start()
            {
                // Ищем по сети хабы.
                CreateClients();
                _webServer = WebApp.Start<Startup>(_options);
            }
            /// <summary>
            /// Пройти по локальной сети и установить соединение с уже запущенными серверами
            /// </summary>
            private void CreateClients()
            {
                
                var baseIP = MyIP.Substring(0, 1 + MyIP.LastIndexOf("."));
                var pings = new Ping(baseIP);
                pings.RunPingSweepAsync().Wait();
                var IPs = pings.Urls;
                IPs.AsParallel().ForAll(ip =>
                {
                    try
                    {
                        var clientUrl = $"http://{ip}:{Port}";
                        var client = new NotificationClient(clientUrl, OtherServersKeys);
                        Clients.Add(client);
                    }
                    catch
                    {
                        // Нет соединения, ну и ладно.
                    }
                });
            }

            public void Stop()
            {
                _webServer?.Dispose();
            }
        }
    }
}
