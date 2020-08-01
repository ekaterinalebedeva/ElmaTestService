using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService.Broadcast
{
    class NotificationClient : IDisposable
    {
        private HubConnection _hubConnection;
        public NotificationClient(string url)
        {
            SetConnectionAsync(url).Wait();
            Console.WriteLine($"Established connection to {url}.");

        }

        private async Task<bool> SetConnectionAsync(string url)
        {
            _hubConnection = new HubConnection(url);
            IHubProxy notfificationHubProxy = _hubConnection.CreateHubProxy("NotificationHub");
            //notfificationHubProxy.On<string>("AddKey", stock => Console.WriteLine("Stock update for {0} new price {1}", stock.Symbol, stock.Price));
            //ServicePointManager.DefaultConnectionLimit = 10;
            await _hubConnection.Start();
            return true;
        }

        public void Dispose()
        {
            _hubConnection.Dispose();
            //TODO throw new NotImplementedException();
        }
    }
}
