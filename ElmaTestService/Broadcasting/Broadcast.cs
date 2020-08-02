﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using ElmaTestService.Models;

namespace ElmaTestService.Broadcasting
{
    public class Broadcast
    {
        public readonly IStoragable<string, string> MyStorage = Program.MyStorage;
        
        /// <summary>
        ///  Singleton instance
        /// </summary>
        private readonly static Lazy<Broadcast> _instance = new Lazy<Broadcast>(() => new Broadcast(GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients));

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
            Console.WriteLine("server: отправил ключи");
        }

        public void AddKey(object key)
        {
            Clients.All.AddKey(key);
            Console.WriteLine($"server: клиенты, добавьте мой ключ {key}");
        }
        public void DeleteKey(object key)
        {
            Clients.All.DeleteKey(key);
            Console.WriteLine($"server: клиенты, удалите ключ {key}");
        }

        public void AddClientKey(object key, string url)
        {
            Program.OtherServersKeys[key as string] = url;
            Console.WriteLine($"server: по запросу добавил ключ {key} клиента {url}");
        }
        public void DeleteClientKey(string key)
        {
            Program.OtherServersKeys.TryRemove(key, out var value);
            Console.WriteLine($"server: по запросу удалил ключ {key} клиента");
        }
    }
}
