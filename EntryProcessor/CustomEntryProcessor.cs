using System;

using Alachisoft.NCache.Runtime.Processor;

using Entities;

namespace EntryProcessor
{
    [Serializable]
    public class CustomEntryProcessor : IEntryProcessor
    {
        //Ignore Lock
        public bool IgnoreLock()
        {
            return true;        //Yes
        }


        public object Process(IMutableEntry entry, params object[] arguments)
        {
            var customer = new Customer()
            {
                CustomerId = "CISRG",
                ContactName = "Device",
                CompanyName = "Astralis",
                Address = "Denmark",
                ContactTitle = "Awper"
            };

            if (entry.Key.Equals("Customer:GHOST"))
            {
                Console.WriteLine("Key: " + entry.Key + " --- from Entry Processor");

                customer = new Customer()
                {
                    CustomerId = "KRIMZ",
                    ContactName = "Flusha",
                    CompanyName = "Fnatic",
                    Address = "Portugal",
                    ContactTitle = "Awper"
                };

                entry.Value = customer;
                return customer;
            }
            else if(entry.Value is Customer)
            {
                Console.WriteLine("No Implementation provided for Customer Object -- Entry Processor");
            }
            else
            {
                Console.WriteLine("Entry doesn't exist");
            }

            return customer;
        }
    }
}
