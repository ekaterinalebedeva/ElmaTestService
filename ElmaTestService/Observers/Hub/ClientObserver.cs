using ElmaTestService.Broadcasting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService.Observers.Hub
{
    public class ClientObserver<T> : IObserver<T>
    {
        private IEnumerable<NotificationClient> _clients;
        public ClientObserver(IEnumerable<NotificationClient> clients)
        {
            _clients = clients;
        }
        public void OnAdd(T handle)
        {
            _clients.AsParallel().ForAll(client =>
            {
                client.AddKey(handle);
            });
        }

        public void OnDelete(T handle)
        {
            _clients.AsParallel().ForAll(client =>
            {
                client.DeleteKey(handle);
            });
        }

        public void OnUpdate(T handle)
        {
            //TODO пока не требуется
        }
    }
}
