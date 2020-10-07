using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.MapReduce;

namespace MapReduce
{
    [Serializable]
    public class Combiner : ICombiner
    {
        private object temp = null;
        public void Dispose()
        {

        }

        public void BeginCombine()
        {

        }

        public void Combine(object value)
        {
            temp = value;
        }

        public object FinishChunk()
        {
            return temp;
        }
    }
}