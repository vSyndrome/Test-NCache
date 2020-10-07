using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.MapReduce;

namespace MapReduce
{
    [Serializable]
    public class KeyFilter : IKeyFilter
    {
        public bool FilterKey(object key)
        {
            return ((string) key).Contains("Customer:L");

        }
    }
}
