using System;

using Alachisoft.NCache.Runtime.Caching;

using Entities;
using NCache;

namespace NCacheLoader
{
    class TestClass
    {
        private static void Main(string[] args)
        {
            //----------------------------------------------------------------------------------------------
            //Initializing some temporary variables for testing.

            string cacheName = "SampleCache";
            string filePath = @"F://timestamps.txt";
            string customerKey = "Customer:GHOST";
            string topicName = "CustomerTopic";
            string subscriptionName = "CustomerSubscription";
            string groupName1 = "Good Customers";
            string groupName2 = "Bad Customers";

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
            nameTag.Add("FlashSaleDiscount", 0.5);

            Tag[] newTags = new Tag[5]
            {
                new Tag("Type 1 Customers"),
                new Tag("Type 2 Customers"),
                new Tag("Type 3 Customers"),
                new Tag("Type 4 Customers"),
                new Tag("Type 5 Customers")
            };

            //----------------------------------------------------------------------------------------------
            //Callouts

            //-----------------------------------------------
            //Client Side

            Program program = new Program(cacheName);

            //program.ClearCache();
            program.GetCount();
            //program.InCache(customerKey);

            program.Add(customerKey, customer);

            //program.UpdateCacheItem(customerKey, customer);
            //program.ClearCache();
            
            program.GetCount();

            var item = program.GetCacheItem(customerKey);
            Console.WriteLine("CustomerId: " + item.GetValue<Customer>().CustomerId);
            Console.WriteLine("ContactName: " + item.GetValue<Customer>().ContactName);
            Console.WriteLine("CompanyName: " + item.GetValue<Customer>().CompanyName);
            Console.WriteLine("ContactTitle: " + item.GetValue<Customer>().ContactTitle);
            Console.WriteLine("Address: " + item.GetValue<Customer>().Address);

            //program.AddBulk(keys, customerArray);

            //var item = program.GetBulk(keys);
            //foreach (var entry in item)
            //{
            //    Console.WriteLine("Customer: " + entry.ContactName + " retrieved by bulk call");
            //}

            //program.AddGroup(KeysFirstHalf,groupName1);
            //program.AddGroup(KeysSecondHalf,groupName2);

            //program.RemoveGroup(KeysSecondHalf);

            //program.AddTags(KeysSecondHalf,newTags);

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



            //-----------------------------------------------
            //Server Side


            //program.EntryProcessor(customerKey, customer);






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


            Console.WriteLine();
            Console.WriteLine("End of the program");
        }
    }
}
