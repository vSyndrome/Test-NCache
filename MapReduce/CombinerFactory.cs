using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.MapReduce;

namespace MapReduce
{
    [Serializable]
    public class CombinerFactory : ICombinerFactory
    {
        public IDictionary<string, ICombiner> combiners = new Dictionary<string, ICombiner>(StringComparer.CurrentCultureIgnoreCase);

        public ICombiner Create(object key)
        {
            if (!combiners.ContainsKey(key as string))
            {
                combiners.Add(key as string, new Combiner());
            }
            return combiners[key as string];
        }
    }

}
