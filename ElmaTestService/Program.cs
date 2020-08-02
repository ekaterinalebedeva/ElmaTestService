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
        /// Порт. Можно задать из параметров командной строки
        /// </summary>
        public static string Port { get; set; } = ConfigurationManager.AppSettings.Get("Port");
        /// <summary>
        /// Свой URL в локальной сети
        /// </summary>
        public static string MyUrl { get; set; }
        static void Main(string[] args)
        {
            SetUrl();

            // Присоединяем наблюдателя-сервера, который будет отсылать уведомления клиентам.
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
        /// <summary>
        /// Пытаемся получить свой IP в локальной сети
        /// </summary>
        static void SetUrl()
        {
            string hostName = Dns.GetHostName(); 
            Console.WriteLine($"hostName {hostName}");
            MyUrl = Dns.GetHostEntry(hostName).AddressList.LastOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            if (string.IsNullOrEmpty(MyUrl))
            {
                throw new Exception("Не удалось получить адрес в локальной сети");
            }
            MyUrl = $"http://{MyUrl}:{Port}";
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
                var urls = new List<string>();
                var _ipPrefix = MyUrl.Split('.');
                for (int i = 0; i <= 255; i++)
                {
                    _ipPrefix[3] = $"{i}:{Port}";
                    var scanIP = string.Join(".", _ipPrefix);
                    if (scanIP == MyUrl) continue;
                    urls.Add(scanIP);
                }
                urls.AsParallel().ForAll(url =>
                {
                    try
                    {
                        Console.WriteLine(url);
                        var client = new NotificationClient(url, OtherServersKeys);
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
