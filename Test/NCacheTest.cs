using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.Caching;
using NCache;

namespace Test
{
    class NCacheTest{

        public static void Main(string[] args)
        {
            //----------------------------------------------------------------------------------------------
            //Initializing some temporary variables for testing.

            string cacheName = "SampleCache";
            string filePath = @"D://test.txt";
            string customerKey = "Customer:0";
            string topicName = "CustomerTopic";
            string subscriptionName = "CustomerSubscription";
            string groupName1 = "Good Customers";
            string groupName2 = "Bad Customers";

            var customer = new Customer
            {
                CustomerId = 0,
                ContactName = "Nikola Kovac",
                CompanyName = "Faze Clan",
                ContactTitle = "Lurker",
                Address = "Sweden"
            };

            var customer1 = new Customer
            {
                CustomerId = 1,
                ContactName = "David Jones",
                CompanyName = "Google",
                ContactTitle = "Developer",
                Address = "32-Newel Road, Palo Alto"
            };

            var customer2 = new Customer
            {
                CustomerId = 2,
                ContactName = "Mike Harry",
                CompanyName = "Uber",
                ContactTitle = "Barrister",
                Address = "Sydney, Australia"
            };

            var customer3 = new Customer
            {
                CustomerId = 3,
                ContactName = "Kratos",
                CompanyName = "WarLord",
                ContactTitle = "Adventurer",
                Address = "Nowhere"
            };

            var customer4 = new Customer
            {
                CustomerId = 4,
                ContactName = "Ezio Auditore",
                CompanyName = "Ubisoft",
                ContactTitle = "Assassin",
                Address = "Paris, France"
            };

            var customer5 = new Customer
            {
                CustomerId = 5,
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


            string[] keys = new string[5];

            for (int i = 0; i < 5; i++)
            {
                keys[i] = "Customer:" + i + 1;
            }

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

            Program program = new Program(cacheName);

            //program.ClearCache();
            //var item = program.Get($"Customer:{80}");
            //Console.WriteLine("CustomerId: " + item.CustomerId + " --------- retrieved through loaded data");
            //Console.WriteLine("ContactName: " + item.ContactName + " --------- retrieved through loaded data");
            //Console.WriteLine("CompanyName: " + item.CompanyName + " --------- retrieved through loaded data");
            //Console.WriteLine("ContactTitle: " + item.ContactTitle + " --------- retrieved through loaded data");
            //Console.WriteLine("Address: " + item.Address + " --------- retrieved through loaded data");

            //program.Update(customerKey, customer);

            //program.AddBulk(keys,customerArray);

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




            //----------------------------------------------------------------------------------------------
            //Write timestamps to a file

            //using (StreamWriter writer = new StreamWriter("D://test.txt"))
            //{
            //    int counter = 0;
            //    while (counter < 10000)
            //    {
            //        writer.WriteLine("TimeStamp:  Hour: " + DateTime.Now.Hour + ",  Minute: " + DateTime.Now.Minute + ",  Second: " + DateTime.Now.Second + ", Millisecond: " + DateTime.Now.Millisecond);
            //        counter++;
            //    }
            //}

            //string readText = File.ReadAllText("D://test.txt");
            //Console.WriteLine(readText);


            Console.WriteLine();
            Console.WriteLine("End of the program");

        }

    }
}
