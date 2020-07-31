using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService
{
    public interface IStorage<TItem>
    {
        bool Add(string key, TItem value);
        bool Remove(string key);
        bool TryGetByKey(string key, out TItem value);
    }
    public class Storage<TItem> : IStorage<TItem>
    {
        private ConcurrentDictionary<string, TItem> _values;
        public ConcurrentDictionary<string, TItem> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new ConcurrentDictionary<string, TItem>();
                }

                return _values;
            }
        }
        public bool Add(string key, TItem value)
        {
            Values[key] = value;
            return true;
        }

        public bool Remove(string key) => Values.TryRemove(key, out var value);

        public bool TryGetByKey(string key, out TItem value) => Values.TryGetValue(key, out value);
    }
}
