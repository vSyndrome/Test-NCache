using System;

using Alachisoft.NCache.Runtime.Caching;

using Entities;
using NCache;

namespace NCacheTester
{
    internal static class Tester
    {
        private static void NewLine()
        {
            Console.WriteLine();
        }

        private static void Main(string[] args)
        {
            //----------------------------------------------------------------------------------------------
            //Initializing some temporary variables for testing.

            string cacheName = "NiftyCache";
            string cacheName2 = "MirrorCache";
            string cacheName3 = "PartitionedCache";
            string clientCacheName2 = "MirrorClientCache";
            string clientCacheName = "ElectronicCache";
            string clientCacheName3 = "P_ClientCache";
#pragma warning disable CS0219 // The variable 'filePath' is assigned but its value is never used
            string filePath = @"F://timestamps.txt";
#pragma warning restore CS0219 // The variable 'filePath' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'customerKey' is assigned but its value is never used
            string customerKey = "Customer:VAMP";
#pragma warning restore CS0219 // The variable 'customerKey' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'topicName' is assigned but its value is never used
            string topicName = "CustomerTopic";
#pragma warning restore CS0219 // The variable 'topicName' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'subscriptionName' is assigned but its value is never used
            string subscriptionName = "CustomerSubscription";
#pragma warning restore CS0219 // The variable 'subscriptionName' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'groupName1' is assigned but its value is never used
            string groupName1 = "GoodCustomers";
#pragma warning restore CS0219 // The variable 'groupName1' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'groupName2' is assigned but its value is never used
            string groupName2 = "BadCustomers";
#pragma warning restore CS0219 // The variable 'groupName2' is assigned but its value is never used

            var customer = new Customer
            {
                CustomerId = "NIKO",
                ContactName = "Nikola Kovac",
                CompanyName = "Faze Clan",
                ContactTitle = "Rifler",
                Address = "Sweden"
            };

            var customer1 = new Customer
            {
                CustomerId = "KMNOP",
                ContactName = "David Jones",
                CompanyName = "Google",
                ContactTitle = "Developer",
                Address = "32-Newel Road, Palo Alto"
            };

            var customer2 = new Customer
            {
                CustomerId = "RSTUV",
                ContactName = "Mike Harry",
                CompanyName = "Uber",
                ContactTitle = "Barrister",
                Address = "Sydney, Australia"
            };

            var customer3 = new Customer
            {
                CustomerId = "WXYZZ",
                ContactName = "Kratos",
                CompanyName = "WarLord",
                ContactTitle = "Adventurer",
                Address = "Nowhere"
            };

            var customer4 = new Customer
            {
                CustomerId = "BVCSG",
                ContactName = "Ezio Auditore",
                CompanyName = "Ubisoft",
                ContactTitle = "Assassin",
                Address = "Paris, France"
            };

            var customer5 = new Customer
            {
                CustomerId = "Drome",
                ContactName = "Lara Croft",
                CompanyName = "Square Enix",
                ContactTitle = "Explorer",
                Address = "London, England"
            };

            Customer[] customerArray = new Customer[]
            {
                customer1,
                customer2,
                customer3,
                customer4,
                customer5
            };


            string[] keys = new string[5]
            {
                $"Customer:GHOST",
                $"Customer:RSTUV",
                $"Customer: WXYZZ",
                $"Customer: BVCSG",
                $"Customer: Drome"
            };

            string[] KeysFirstHalf = new string[3]
            {
                keys[0],
                keys[1],
                keys[2]
            };

            string[] KeysSecondHalf = new string[2]
            {
                keys[3],
                keys[4],
            };

            NamedTagsDictionary nameTag = new NamedTagsDictionary();
            nameTag.Add("ImportantCustomer", 0.5);

            Tag[] newTags = new Tag[5]
            {
                new Tag("Type 1 Customers"),
                new Tag("Type 2 Customers"),
                new Tag("Type 3 Customers"),
                new Tag("Type 4 Customers"),
                new Tag("Type 5 Customers")
            };
            

            var product1 = new Product()
            {
                ProductId = 3,
                ProductName = "Cheese",
                UnitPrice = 1,
                UnitsInStock = 50,
                Discontinued = false,
                SupplierId = 17
            };
            var product2 = new Product()
            {
                ProductId = 4,
                ProductName = "Milk",
                UnitPrice = 3,
                UnitsInStock = 50,
                Discontinued = false,
                SupplierId = 17
            };
            var product3 = new Product()
            {
                ProductId = 5,
                ProductName = "Sugar",
                UnitPrice = 6,
                UnitsInStock = 50,
                Discontinued = false,
                SupplierId = 17
            };
            var product4 = new Product()
            {
                ProductId = 7,
                ProductName = "Beef",
                UnitPrice = 8,
                UnitsInStock = 50,
                Discontinued = false,
                SupplierId = 17
            };
            var product5 = new Product()
            {
                ProductId = 8,
                ProductName = "Mutton",
                UnitPrice = 9,
                UnitsInStock = 50,
                Discontinued = false,
                SupplierId = 17
            };
            var product6 = new Product()
            {
                ProductId = 9,
                ProductName = "RTX 3090",
                UnitPrice = 10,
                UnitsInStock = 50,
                Discontinued = false,
                SupplierId = 17
            };

            Product[] products = new Product[6]
            {
                product1,
                product2,
                product3,
                product4,
                product5,
                product6
            };

            int count = 0;
            string[] productKeys = new string[6];

            foreach (var product in products)
            {
                productKeys[count] = $"Product{product.ProductId}";
                count++;
            }

            //----------------------------------------------------------------------------------------------
            //Callouts

            //-----------------------------------------------
            //Client Side

            Program program = new Program(cacheName3,clientCacheName3);

            //program.ClearCache();
            NewLine();
            program.GetCount();
            NewLine();
            program.GetAllKeys();

            program.Get("Chat");

            //program.GetSessionData();
            //program.GetViewStateData();

            //string[] keyset = new string[10000];
            //Customer[] customerss = new Customer[10000];
            //for (int i = 0; i < 10000; i++)
            //{
            //    keyset[i] = $"Customer:{i+4000}";
            //    customerss[i] = customer;
            //}
            //program.AddBulk(keyset,customerss);


            //program.AddBulk(productKeys, products);
            //program.ContinuousQueryForProductUnitPrice(3, 10);

            //program.TestExtensibleDependency("ABCDE");

            //program.AddCacheItem(customerKey, customer2);

            //NewLine();

            //program.AddCacheItem(customerKey, customer5);
            //program.PessimisticLocking(customerKey);
            //program.UpdateCacheItem(customerKey, customer2);

            //program.AddWithGroup(customerKey, customer1, groupName2);
            //program.ContinuousQuerySearchByGroupName(groupName2);

            //program.CheckCache(customerKey);
            //program.Remove(customerKey);
            //program.Add(customerKey, customer);
            //program.Update(customerKey, customer3);
            //program.Remove(customerKey);
            //program.Update(customerKey, customer2);

            //program.AddBulk(keys, customerArray);
            //program.UpdateCacheItem(customerKey, customer);
            //program.ClearCache();

            //program.GetCount();

            //program.AddSqlDependencies();

            //string tempKey = "Customer:LINOD";
            //var item = program.Get(tempKey);
            //if (item!=null)
            //{
            //    Console.WriteLine("CustomerId: " + item.CustomerId);
            //    Console.WriteLine("ContactName: " + item.ContactName);
            //    Console.WriteLine("Mapper Status:" + item.WentThroughMapper.ToString());
            //    Console.WriteLine("CompanyName: " + item.CompanyName);
            //    Console.WriteLine("ContactTitle: " + item.ContactTitle);
            //    Console.WriteLine("Address: " + item.Address);
            //}
            //else
            //{
            //    Console.WriteLine($"Key: {tempKey} doesn't exist in cache");
            //}

            //program.RemoveCacheItem("ABCDE");
            //program.RemoveCacheItem("RSTUV");

            //program.TestExtensibleDependency("ABCDE");

            //var item = program.GetBulk(keys);
            //foreach (var entry in item)
            //{
            //    Console.WriteLine("Customer: " + entry.ContactName + " retrieved by bulk call");
            //}

            //program.AddGroup(KeysFirstHalf, groupName1);
            //program.AddGroup(KeysSecondHalf, groupName2);

            //program.RemoveGroup(KeysSecondHalf);

            //program.AddTags(KeysSecondHalf, newTags);

            //var item = program.GetByAnyTag(newTags);

            //foreach (var entry in item)
            //{
            //    Console.WriteLine("Customer: " + entry.ContactName + " retrieved by tags");
            //}

            //program.AddNamedTag(KeysFirstHalf,nameTag);

            //program.SearchByNamedTags();

            //program.CreateTopic(topicName);
            //program.PublicMessagesToTopics(topicName, customer);
            //program.SubscribeForTopicMessages(topicName);

            //program.DeleteTopic(topicName);

            //program.DurableSubscription(topicName, subscriptionName);
            //program.PatternBasedSubscription(topicName);
            //program.PatternBasedDurableSubscription(topicName, subscriptionName);
            //program.PatternBasedSubscriptionWithFailureNotification(topicName);


            //----------------------------------------------------------------------------------------------
            //Pub Sub PowerShell Commands

            //Get-Topics -CacheName SampleCache
            //Get-Topics -CacheName SampleCache -Detail
            //Get-Topics -CacheName SampleCache -ShowAll

            //program.CreateTopic(topicName);
            //program.PublicMessagesToTopics(topicName, customer3);
            //program.SubscribeForTopicMessages(topicName);
            //program.SubscribeForTopicMessages(topicName);
            //program.SubscribeForTopicMessages(topicName);
            //program.PublicMessagesToTopics(topicName, customer4);


            //-----------------------------------------------
            //Server Side

            //program.EntryProcessor(customerKey, customer);
            //program.TestAggregate();
            //program.MapReduceExecutor();

            //----------------------------------------------------------------------------------------------
            //Write timestamps to a file

            //using (StreamWriter writer = new StreamWriter("F://timestamps.txt"))
            //{
            //    int counter = 0;
            //    while (counter < 10000)
            //    {
            //        writer.WriteLine("TimeStamp:  Hrs: " + DateTime.Now.Hour + ",  Mins: " + DateTime.Now.Minute + ",  Sec: " + DateTime.Now.Second + ", mSec: " + DateTime.Now.Millisecond);
            //        counter++;
            //    }
            //}

            //string readText = File.ReadAllText("F://timestamps.txt");
            //Console.WriteLine(readText);

            NewLine();
            Console.WriteLine("End of the program");
        }

    }
}
