using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.MapReduce;

namespace MapReduce
{
    [Serializable]
    public class ReducerFactory : IReducerFactory
    {
        IDictionary<string, IReducer> reducers = new Dictionary<string, IReducer>(StringComparer.CurrentCultureIgnoreCase);

        public IReducer Create(object key)
        {
            if (!reducers.ContainsKey(key as string))
            {
                // If you want to map output against a different key,
                // specify a different key in constructor

                reducers.Add(key as string, new Reducer(key as string));
            }

            return reducers[key as string];
        }
    }
}
