using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace ElmaTestService.Broadcasting
{
    class NotificationClient : IDisposable
    {
        private HubConnection _hubConnection;

        private readonly Broadcast _broadcast;

        public NotificationClient(string url) : this(Broadcast.Instance, url) { }

        public NotificationClient(Broadcast broadcast, string url)
        {
            _broadcast = broadcast; 
            var task = SetConnectionAsync(url);
            task.Wait();
            if (!task.Result)
            {
                throw new Exception($"Не удалось подключиться к {url}.");
            }
        }

        private async Task<bool> SetConnectionAsync(string url)
        {
            _hubConnection = new HubConnection(url);
            IHubProxy notificationHubProxy = _hubConnection.CreateHubProxy("NotificationHub");
            notificationHubProxy.On<IEnumerable<string>>("GetAllKeys", key => Console.WriteLine($"client: Пришли ключи {string.Join("     ",key)}"));
            //ServicePointManager.DefaultConnectionLimit = 100;
            await _hubConnection.Start();
            if (_hubConnection.State == ConnectionState.Connected)
            {
                Console.WriteLine($"client: Established connection to {url}.");
            }
            return true;
        }

        public void Dispose()
        {
            _hubConnection.Dispose();
        }
    }
}
