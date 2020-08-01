using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace ElmaTestService.Broadcast
{
    class NotificationClient : IDisposable
    {
        private HubConnection _hubConnection;
        public NotificationClient(string url)
        {
            SetConnectionAsync(url).Wait();
            //var task = SetConnectionAsync(url);
            //task.Wait();
            //if (task.Result)
            {
                
            }
        }

        private async Task<bool> SetConnectionAsync(string url)
        {
            _hubConnection = new HubConnection(url);
            IHubProxy notificationHubProxy = _hubConnection.CreateHubProxy("NotificationHub");
            notificationHubProxy.On<string>("GetAllKeys", key => Console.WriteLine($"Пришли ключи {key}"));
            //ServicePointManager.DefaultConnectionLimit = 100;
            await _hubConnection.Start();
            if (_hubConnection.State == ConnectionState.Connected)
            {
                Console.WriteLine($"Established connection to {url}.");
                await notificationHubProxy.Invoke("GetAllKeys");
            }
            return true;
        }

        public void Dispose()
        {
            _hubConnection.Dispose();
            //TODO throw new NotImplementedException();
        }
    }
}
