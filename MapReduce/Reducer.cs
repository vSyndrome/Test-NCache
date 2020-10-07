using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.MapReduce;

namespace MapReduce
{
    [Serializable]
    public class Reducer : IReducer
    {
        private string outputKey;
        private KeyValuePair outputMapEntry;

        public Reducer(string key)
        {
            outputKey = key;
        }

        public void Dispose()
        {
            
        }

        public void BeginReduce()
        {
            outputMapEntry = new KeyValuePair();
            outputMapEntry.Key = outputKey;
        }

        public void Reduce(object value)
        {
            
        }

        public KeyValuePair FinishReduce()
        {
            outputMapEntry.Value = true;
            return outputMapEntry;
        }
    }
}
