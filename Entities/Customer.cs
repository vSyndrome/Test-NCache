using System;
using Alachisoft.NCache.Runtime.Serialization;
using Alachisoft.NCache.Runtime.Serialization.IO;

namespace Entities
{
    [Serializable]
    public class Customer
    {
        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public bool WentThroughMapper { get; set; } = false;
        public string ContactTitle { get; set; }
        public string Address { get; set; }
    }
}
