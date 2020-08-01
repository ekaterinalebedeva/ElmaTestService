using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService.Observers
{
    public class Observable<T> : AObservable<T>
    {
        public override bool OnAdd(T handle)
        {
            _observers.ForEach(observer => observer.OnAdd(handle));
            return true;
        }

        public override bool OnDelete(T handle)
        {
            _observers.ForEach(observer => observer.OnDelete(handle));
            return true;
        }

        public override bool OnUpdate(T handle)
        {
            _observers.ForEach(observer => observer.OnUpdate(handle));
            return true;
        }
    }
}
