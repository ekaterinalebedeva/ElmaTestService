using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService.Broadcast
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
        public IEnumerable<string> GetAllKeys()
        {
            return _broadcast.MyStorage.Values.Keys.ToList();
        }
    }

    public class Broadcast
    {
        public readonly Storage<string> MyStorage = Startup.MyStorage;
        // Singleton instance
        private readonly static Lazy<Broadcast> _instance = new Lazy<Broadcast>(() => new Broadcast(GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients));

        private readonly ConcurrentDictionary<string, string> _otherStorage = new ConcurrentDictionary<string, string>();

        private readonly object _updateStorageLock = new object();

        //stock can go up or down by a percentage of this factor on each change
        private readonly double _rangePercent = .002;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        //private readonly Timer _timer;
        private volatile bool _updatingStorage = false;

        private Broadcast(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _otherStorage.Clear();
            var myKeys = Startup.MyStorage.Values.Keys.ToList();
            myKeys.ForEach(myKey => _otherStorage[myKey]="");

            //_timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);
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

        public IEnumerable<string> GetAllKeys()
        {
            return _otherStorage.Keys;
        }

        //private void UpdateStockPrices(object state)
        //{
        //    lock (_updateStorageLock)
        //    {
        //        if (!_updatingStorage)
        //        {
        //            _updatingStorage = true;

        //            foreach (var stock in _otherStorage.Values)
        //            {
        //                if (TryUpdateStockPrice(stock))
        //                {
        //                    BroadcastStockPrice(stock);
        //                }
        //            }

        //            _updatingStorage = false;
        //        }
        //    }
        //}

        //private bool TryUpdateStockPrice(Stock stock)
        //{
        //    // Randomly choose whether to update this stock or not
        //    var r = _updateOrNotRandom.NextDouble();
        //    if (r > .1)
        //    {
        //        return false;
        //    }

        //    // Update the stock price by a random factor of the range percent
        //    var random = new Random((int)Math.Floor(stock.Price));
        //    var percentChange = random.NextDouble() * _rangePercent;
        //    var pos = random.NextDouble() > .51;
        //    var change = Math.Round(stock.Price * (decimal)percentChange, 2);
        //    change = pos ? change : -change;

        //    stock.Price += change;
        //    return true;
        //}

        private void BroadcastStockPrice(string key)
        {
            Clients.All.updateStockPrice(key);
        }

    }
}
