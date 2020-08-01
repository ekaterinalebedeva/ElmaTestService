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
        public void OnAdd(T handle)
        {
            Program.Clients.ForEach(client =>
            {
                client.AddKey(handle);
            });
        }

        public void OnDelete(T handle)
        {
            Program.Clients.ForEach(client =>
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
