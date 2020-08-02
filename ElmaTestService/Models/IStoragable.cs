using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaTestService.Models
{
    public interface IStoragable<TKey, TValue>
    {
        bool Add(TKey key, TValue value);
        bool Remove(TKey key);
        bool TryGetByKey(TKey key, out TValue value);
        IEnumerable<TKey> GetAllKeys();
    }
}
