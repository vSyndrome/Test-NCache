using System;

using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.MapReduce;

using Entities;

namespace MapReduce
{
    [Serializable]
    public class Mapper : IMapper
    {
        private ICache cache;

        public Mapper(string cacheName, CacheConnectionOptions cacheConnectionOptions)
        {
            cache = CacheManager.GetCache(cacheName,cacheConnectionOptions);
        }

        public void Map(object key, object value, IOutputMap context)
        {
            if (value is Customer tempCustomer)
            {
                string myKey = (string) key;
                tempCustomer.WentThroughMapper = true;
                var cacheItem = new  CacheItem(tempCustomer);
                Console.WriteLine("Through Mapper");
                cache.Insert(myKey, cacheItem);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
