using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmaTestService.Broadcasting;

namespace ElmaTestService.Observers.Hub
{
    class HubObserver<T> : IObserver<T>
    {
        private Broadcast _broadcast;
        public HubObserver() : this(Broadcast.Instance) { }
        public HubObserver(Broadcast broadcast)
        {
            _broadcast = broadcast;
        }

        public void OnAdd(T handle)
        {
            _broadcast.AddKey(handle);
        }

        public void OnDelete(T handle)
        {
            _broadcast.DeleteKey(handle);
        }

        public void OnUpdate(T handle)
        {
            //TODO пока не требуется
        }
    }
}
