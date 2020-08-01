using ElmaTestService.Observers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService
{
    public interface IStorage<TKey, TValue>
    {
        bool Add(TKey key, TValue value);
        bool Remove(TKey key);
        bool TryGetByKey(TKey key, out TValue value);
        IEnumerable<TKey> GetAllKeys();
    }
    public class Storage<TKey, TValue> : Observable<TKey>, IStorage<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> _values = new ConcurrentDictionary<TKey, TValue>();

        public bool Add(TKey key, TValue value)
        {
            _values[key] = value;
            OnAdd(key);
            return true;
        }

        public bool Remove(TKey key)
        {
            var result = _values.TryRemove(key, out var value);
            if (result)
            {
                OnDelete(key);
            }
            return result;
        }

        public bool TryGetByKey(TKey key, out TValue value) => _values.TryGetValue(key, out value);

        public IEnumerable<TKey> GetAllKeys()
        {
            return _values.Keys;
        }
    }


}
