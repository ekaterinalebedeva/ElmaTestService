using ElmaTestService.Observers;
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
        IEnumerable<string> GetAllKeys();
    }
    public class Storage<TItem> : Observable<string>, IStorage<TItem>
    {
        private ConcurrentDictionary<string, TItem> _values = new ConcurrentDictionary<string, TItem>();

        public bool Add(string key, TItem value)
        {
            _values[key] = value;
            return true;
        }

        public bool Remove(string key) => _values.TryRemove(key, out var value);

        public bool TryGetByKey(string key, out TItem value) => _values.TryGetValue(key, out value);

        public IEnumerable<string> GetAllKeys()
        {
            return _values.Keys;
        }
    }


}
