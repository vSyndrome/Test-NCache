using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Aggregation;

namespace Aggregate
{
    [Serializable]
    public class AggregateClass : IAggregator
    {
        private string var;

        public AggregateClass(string function)
        {
            var = function;
        }

        public object Aggregate(object value)
        {
            return Calculate(value);
        }

        public object AggregateAll(object value)
        {
            return Calculate(value);
        }

        private object Calculate(object value)
        {
            switch (var)
            {
                case "COUNT":
                    return value;
                default:
                    return 0;
            }
        }
    }


}
