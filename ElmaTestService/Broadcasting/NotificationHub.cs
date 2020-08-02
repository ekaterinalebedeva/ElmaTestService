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
    [HubName("NotificationHub")]
    public class NotificationHub : Hub
    {
        private readonly Broadcast _broadcast;

        public NotificationHub() : this(Broadcast.Instance) { }

        public NotificationHub(Broadcast broadcast)
        {
            _broadcast = broadcast;
        }

        public override Task OnConnected()
        {
            _broadcast.OnClientConnected(Clients.Caller);
            return base.OnConnected();
        }

        public void AddClientKey(string key, string url)
        {
            _broadcast.AddClientKey(key, url);
        }
        public void DeleteClientKey(string key)
        {
            _broadcast.DeleteClientKey(key);
        }
    }
}
