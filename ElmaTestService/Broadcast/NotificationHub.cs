using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService.Broadcast
{
    [HubName("NotificationHub")]
    public class NotificationHub : Hub
    {
        private readonly Storage<string> _storage = Startup.MyStorage;

        public IEnumerable<string> GetAllKeys()
        {
            return _storage.Values.Keys.ToList();
        }
    }

    //public class Broadcaster
    //{
    //    private readonly static Lazy<Broadcaster> _instance =
    //        new Lazy<Broadcaster>(() => new Broadcaster());
    //    private readonly IHubContext _hubContext;
    //    public Broadcaster()
    //    {
    //        // Save our hub context so we can easily use it 
    //        // to send to its connected clients
    //        _hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
    //        _model = new ShapeModel();
    //        _modelUpdated = false;
    //        // Start the broadcast loop
    //        _broadcastLoop = new Timer(
    //            BroadcastShape,
    //            null,
    //            BroadcastInterval,
    //            BroadcastInterval);
    //    }
    //}
}
