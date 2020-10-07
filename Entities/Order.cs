using System;

namespace Entities
{
    [Serializable]
    public class Order
    {
        public string OrderId { get; set; }
        public int CustomerId { get; set; }
        public string ShipAddress { get; set; }
        public string Freight { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
