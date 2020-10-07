using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.Aggregation;
using Entities;

namespace Aggregate
{
    [Serializable]
    public class ValueExtractor : IValueExtractor
    {
        public object Extract(object value)
        {
            return value;
        }
    }
}
