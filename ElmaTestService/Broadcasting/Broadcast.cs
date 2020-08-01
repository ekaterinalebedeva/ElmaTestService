using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService.Broadcasting
{
    public class Broadcast
    {
        public readonly IStorage<string> MyStorage = Startup.MyStorage;
        /// <summary>
        ///  Singleton instance
        /// </summary>
        private readonly static Lazy<Broadcast> _instance = new Lazy<Broadcast>(() => new Broadcast(GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients));
        /// <summary>
        /// Словарь с ключами, которые находятся на других серверах
        /// </summary>
        private readonly ConcurrentDictionary<object, string> _otherStorage = new ConcurrentDictionary<object, string>();


        private Broadcast(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public static Broadcast Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        /// <summary>
        /// Действия при установлении подключения нового клиента к серверу
        /// В данном случае отправляем ключи, которые есть на этом сервере
        /// </summary>
        public void OnClientConnected(dynamic client)
        {
            client.GetAllKeys(MyStorage.GetAllKeys());
            Console.WriteLine("server:отправил ключи");
        }

        public void AddKey(object key)
        {
            Clients.All.AddKey(key);
            Console.WriteLine($"server:добавил ключ {key}");
        }
        public void DeleteKey(object key)
        {
            Clients.All.DeleteKey(key);
            Console.WriteLine($"server:удалил ключ {key}");
        }
    }
}
