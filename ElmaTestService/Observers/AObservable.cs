using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService
{
    public abstract class AObservable<T>
    {
        protected List<IObserver<T>> _observers = new List<IObserver<T>>();

        public void Attach(IObserver<T> observer)
        {
            _observers.Add(observer);
        }
        public void Detach(IObserver<T> observer)
        {
            _observers.Remove(observer);
        }
        public abstract bool OnAdd(T handle);
        public abstract bool OnUpdate(T handle);
        public abstract bool OnDelete(T handle);
    }
}
