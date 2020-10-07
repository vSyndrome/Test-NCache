using System;

namespace Entities
{
    [Serializable]
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitPrice { get; set; }
        public int SupplierId { get; set; }
        public bool Discontinued { get; set; }
        public int UnitsInStock { get; set; }
    }
}
