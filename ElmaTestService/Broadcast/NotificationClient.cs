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
        public NotificationClient(string url)
        {
            SetConnectionAsync(url);

        }
         
        private async void SetConnectionAsync(string url)
        {
            using (var hubConnection = new HubConnection(url))
            {
                IHubProxy notfificationHubProxy = hubConnection.CreateHubProxy("NotificationHub");
                //notfificationHubProxy.On<string>("AddKey", stock => Console.WriteLine("Stock update for {0} new price {1}", stock.Symbol, stock.Price));
                //ServicePointManager.DefaultConnectionLimit = 10;
                await hubConnection.Start();
            }
        }

        public void Dispose()
        {
            //TODO throw new NotImplementedException();
        }
    }
}
