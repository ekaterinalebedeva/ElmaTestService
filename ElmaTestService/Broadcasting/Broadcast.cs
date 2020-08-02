using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using ElmaTestService.Models;
using System.Collections;
using System.Collections.Generic;

namespace ElmaTestService.Broadcasting
{
    public class Broadcast
    {
        private static Object _mutex = new Object();
        public readonly IStoragable<string, string> _storage;
        public readonly IDictionary<string, string> _otherServersKeys;
        /// <summary>
        ///  Singleton instance
        /// </summary>
        private static Broadcast _instance;

        private Broadcast(IHubConnectionContext<dynamic> clients, IStoragable<string, string> storage, IDictionary<string, string> otherServersKeys)
        {
            Clients = clients;
            _storage = storage;
            _otherServersKeys = otherServersKeys;
        }

        public static Broadcast GetInstance(IStoragable<string, string> storage, IDictionary<string, string> otherServersKeys)
        {
            if (_instance == null)
            {
                lock (_mutex)
                {
                    if (_instance == null)
                    {
                        _instance = new Broadcast(GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients, storage, otherServersKeys);
                    }
                }
            }

        return _instance;
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
            client.GetAllKeys(_storage.GetAllKeys());
            Console.WriteLine("server: отправил ключи");
        }
        /// <summary>
        /// Отправить сообщение о добавлении ключа на хабе
        /// </summary>
        /// <param name="key"></param>
        public void AddKey(object key)
        {
            Clients.All.AddKey(key);
            Console.WriteLine($"server: клиенты, добавьте мой ключ {key}");
        }
        /// <summary>
        /// Отправить сообщение об удалении ключа
        /// </summary>
        /// <param name="key"></param>
        public void DeleteKey(object key)
        {
            Clients.All.DeleteKey(key);
            Console.WriteLine($"server: клиенты, удалите ключ {key}");
        }
        /// <summary>
        /// Клиент прислал сообщение, что добавил ключ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="url"></param>
        public void AddClientKey(object key, string url)
        {
            _otherServersKeys[key as string] = url;
            Console.WriteLine($"server: по запросу добавил ключ {key} клиента {url}");
        }
        /// <summary>
        /// Клиент прислал сообщение, что удалил ключ
        /// </summary>
        /// <param name="key"></param>
        public void DeleteClientKey(string key)
        {
            _otherServersKeys.Remove(key);
            Console.WriteLine($"server: по запросу удалил ключ {key} клиента");
        }
    }
}
