using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService
{
    /// <summary>
    /// Сканирующий локальную сеть класс
    /// </summary>
    public class Ping
    {
        private string _baseIP;
        private int _startIP = 1;
        private int _stopIP = 255;
        private string _ip;

        private int _timeout = 100;
        private int _nFound = 0;
        public List<string> Urls { get; } = new List<string>();

        static object _lockObj = new object();
        Stopwatch _stopWatch = new Stopwatch();
        TimeSpan ts;

        public Ping(string baseIP)
        {
            _baseIP = baseIP;
        }
        public async Task RunPingSweepAsync()
        {
            _nFound = 0;

            var tasks = new List<Task>();

            _stopWatch.Start();

            for (int i = _startIP; i <= _stopIP; i++)
            {
                _ip = _baseIP + i.ToString();

                System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                var task = PingAndUpdateAsync(p, _ip);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks).ContinueWith(t =>
            {
                _stopWatch.Stop();
                ts = _stopWatch.Elapsed;
                Console.WriteLine($"{_nFound} IPs. Время: {ts}");
            });
        }

        private async Task PingAndUpdateAsync(System.Net.NetworkInformation.Ping ping, string ip)
        {
            var reply = await ping.SendPingAsync(ip, _timeout);

            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                lock (_lockObj)
                {
                    _nFound++;
                    Urls.Add(ip);
                }
            }
        }
    }
}
