using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService
{
    public interface IObserver<T>
    {
        void OnAdd(T handle);
        void OnDelete(T handle);
        void OnUpdate(T handle);
    }
}
