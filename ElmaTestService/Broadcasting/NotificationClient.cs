using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace ElmaTestService.Broadcasting
{
    /// <summary>
    /// Клиент, который уведомляет об изменениях в хранилище
    /// </summary>
    class NotificationClient : IDisposable
    {
        private readonly IDictionary<string, string> _storage;
        private HubConnection _hubConnection;
        private IHubProxy _notificationHubProxy;
        private string _url; 

        public NotificationClient(string url, IDictionary<string, string> storage)
        {
            var task = SetConnectionAsync(url);
            task.Wait();
            if (!task.Result)
            {
                throw new Exception($"Не удалось подключиться к {url}.");
            }

            _storage = storage;
        }

        private async Task<bool> SetConnectionAsync(string url)
        {
            _url = url;
            _hubConnection = new HubConnection($"{url}/signalr");
            _notificationHubProxy = _hubConnection.CreateHubProxy("NotificationHub");
            _notificationHubProxy.On<IEnumerable<string>, string>("GetAllKeys", (keys, serverurl) =>
            {
                foreach (var key in keys)
                {
                    _storage[key] = _url;
                }
                Console.WriteLine($"client: Пришли все ключи от сервера {serverurl}");
            });
            _notificationHubProxy.On<string, string>("AddKey", (key, serverurl) =>
            {
                _storage[key] = serverurl;
                Console.WriteLine($"client: добавил ключ {key} сервера {serverurl}");
            });
            _notificationHubProxy.On<string, string>("DeleteKey", (key, serverurl) =>
            {
                _storage.Remove(key);
                Console.WriteLine($"client: удалил ключ {key} сервера {serverurl}");
            });
           
            await _hubConnection.Start();
            if (_hubConnection.State == ConnectionState.Connected)
            {
                Console.WriteLine($"client: Established connection to {url}.");
                var huburl = _hubConnection.Url;
            }
            return true;
        }

        /// <summary>
        /// Послать хабу команду добавить ключ
        /// </summary>
        /// <param name="key"></param>
        public void AddKey(object key)
        {
            _notificationHubProxy.Invoke("AddClientKey", key, _url);
            Console.WriteLine($"client: сервер, добавь ключ {key} к {_url}");
        }
        /// <summary>
        /// Послать хабу команду удалить ключ
        /// </summary>
        /// <param name="key"></param>
        public void DeleteKey(object key)
        {
            _notificationHubProxy.Invoke("DeleteClientKey", key);
            Console.WriteLine($"client: сервер, удали ключ {key}");
        }
        public void Dispose()
        {
            _hubConnection.Dispose();
        }
    }
}
