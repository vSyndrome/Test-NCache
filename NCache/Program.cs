using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Events;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.MapReduce;
using Alachisoft.NCache.Runtime.Processor;
using Alachisoft.NCache.Runtime.Dependencies;
using Alachisoft.NCache.Runtime.Serialization;
using Alachisoft.NCache.Client.DataTypes.Counter;
using Alachisoft.NCache.Runtime.Serialization.IO;
using Alachisoft.NCache.Runtime.Caching.Messaging;
using Alachisoft.NCache.Client.DataTypes.Collections;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using Aggregate;
using Entities;
using EntryProcessor;
using CustomDependencyProvider;
using MapReduce;

namespace NCache
{
    public class Program : ICompactSerializable
    {
        private static ICache _cache;
        private static CustomEntryProcessor customEntryProcessor;
        private static CacheItemVersion cacheItemVersion;
        private static Customer cachedCustomer;
        private static ExtensibleDependencyProvider extensibleDependency;
        private static Mapper mapper;
        private static MapReduceTask mapReduceTask;
        private static CombinerFactory combinerFactory;
        private static ReducerFactory reducerFactory;
        private static KeyFilter keyFilter;

        private static readonly string connectionString =
            "Data Source=20.200.20.10;Initial Catalog=Northwind-QA;User ID=diyatech;Password=4Islamabad";

        //Compact Serialization Implementations

        public void Deserialize(CompactReader reader)
        {
            cachedCustomer = reader.ReadObject() as Customer;
        }

        public void Serialize(CompactWriter writer)
        {
            writer.WriteObject(cachedCustomer);
        }

        //Constructor

        public Program(string cacheName, string clientCacheName)
        {
            //------------------------------------------------------------------------------------------
            //Initializing Cache

            CacheConnectionOptions cacheConnectionOptions = new CacheConnectionOptions
            {
                ConnectionRetries = 5,
                EnableClientLogs = true,
                LogLevel = LogLevel.Debug,
                //DefaultReadThruProvider = "DBReadThruProvider",
                //DefaultWriteThruProvider = "DBWriteThruProvider",
                //UserCredentials = new Credentials("sal_farooq", "4Islamabad"),
                ClientRequestTimeOut = TimeSpan.FromSeconds(30),
                RetryInterval = TimeSpan.FromSeconds(5),
            };

            CacheConnectionOptions clientCacheConnectionOptions = new CacheConnectionOptions
            {
                ConnectionRetries = 5,
                Mode = Alachisoft.NCache.Client.IsolationLevel.OutProc,
                //UserCredentials = new Credentials("sal_farooq", "4Islamabad"),
                RetryInterval = TimeSpan.FromSeconds(5),
            };

            try
            {
                _cache = CacheManager.GetCache(cacheName, cacheConnectionOptions, clientCacheName, clientCacheConnectionOptions );
                //cache = CacheManager.GetCache(cacheName);
                
                Console.WriteLine("Connected to Cache: " + cacheName);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //-----------------------------------------------------------------------------------------------
            //Another way to Initialize Cache

            //string CacheName = "SampleCache";
            //string ServerIp = "20.200.20.104";
            //int port = 8250;

            //var connection = new CacheConnection(ServerIp, port);
            //CacheManager.StartCache(CacheName, connection);

            //Initialize class objects

            customEntryProcessor = new CustomEntryProcessor();
            
            mapper = new Mapper(cacheName, cacheConnectionOptions);
            combinerFactory = new CombinerFactory();
            reducerFactory = new ReducerFactory();
            mapReduceTask = new MapReduceTask();
            keyFilter = new KeyFilter();

            //-----------------------------------------------------------------------------------------------
            //Binding Events and Registering Callbacks

            //Binding Cache Level Events
            var dataNotificationCallback = new CacheDataNotificationCallback(OnCacheDataModification);
            _cache.MessagingService.RegisterCacheNotification(dataNotificationCallback, EventType.ItemAdded,
                EventDataFilter.DataWithMetadata);
            _cache.MessagingService.RegisterCacheNotification(dataNotificationCallback, EventType.ItemUpdated,
                EventDataFilter.DataWithMetadata);
            _cache.MessagingService.RegisterCacheNotification(dataNotificationCallback, EventType.ItemRemoved,
                EventDataFilter.DataWithMetadata);

            //Binding Management Level Events
            _cache.NotificationService.CacheStopped += OnCacheStopped;
            _cache.NotificationService.CacheCleared += OnCacheCleared;
            _cache.NotificationService.MemberJoined += OnMemberJoined;
            _cache.NotificationService.MemberLeft += OnMemberLeft;

        }

        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //---------------------------------Client Side Features-----------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------


        //Clear Cache
        public void ClearCache()
        {
            try
            {
                _cache.Clear();                    //For Remote Cache
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Check If cache contains a particular Keys
        public void CheckCache(string key)
        {
            if (_cache.Contains(key))
            {
                Console.WriteLine("Key: " + key + " does exist inside the cache");
            }
            else
            {
                Console.WriteLine("Key: " + key + " does not exist inside the cache");
            }
        }


        //Get the count of number of items in Cache
        public void GetCount()
        {
            if (_cache.Count == 0)
            {
                Console.WriteLine("Cache is Empty");
            }
            else
            {
                Console.WriteLine("Cache currently has: " + _cache.Count + " items");
            }
        }


        //Get all keys in the Cache
        public void GetAllKeys()
        {
            int count = 0;
            Console.WriteLine();
            foreach (var key in _cache)
            {
                DictionaryEntry dictionary = new DictionaryEntry();
                count++;
                dictionary = (DictionaryEntry) key;
                Console.WriteLine("Key " + count +": " + dictionary.Key);
            }
        }


        //--------------------------------------------------------------------------------------------
        //Simple CRUD Operations


        //Add Item
        public void Add(string key, Customer customer)
        {
            try
            {
                Console.WriteLine("Before Adding");

                cacheItemVersion = _cache.Add(key, customer);
                cachedCustomer = _cache.Get<Customer>(key);
                Console.WriteLine();
                Console.WriteLine("Customer: " + key + " -- Added");
                Console.WriteLine("Name: " + cachedCustomer.ContactName);
                Console.WriteLine("Company: " + cachedCustomer.CompanyName);
                Console.WriteLine("Address: " + cachedCustomer.Address);
                Console.WriteLine("Item Version: " + cacheItemVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Update Item
        public void Update(string key, Customer customer)
        {
            try
            {
                Console.WriteLine("Before Updating");

                cacheItemVersion = _cache.Insert(key, customer);
                cachedCustomer = _cache.Get<Customer>(key);
                Console.WriteLine();
                Console.WriteLine("Customer: " + key + " -- Updated");
                Console.WriteLine("Name: " + cachedCustomer.ContactName);
                Console.WriteLine("Company: " + cachedCustomer.CompanyName);
                Console.WriteLine("Address: " + cachedCustomer.Address);
                Console.WriteLine("Item Version: " + cacheItemVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Remove Item
        public void Remove(string key)
        {
            try
            {
                Console.WriteLine("Before Removing");

                _cache.Remove(key);
                Console.WriteLine();
                Console.WriteLine("Customer: " + key + " -- Removed");
                cachedCustomer = _cache.Get<Customer>(key);
                if (cachedCustomer == null)
                {
                    Console.WriteLine(key + " doesn't exist");
                }
                else
                {
                    Console.WriteLine(key + " -- was not removed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        //Get Item
        public Customer Get(string key)
        {
            Customer item = null;
            try
            {
                var readThruOptions = new ReadThruOptions {Mode = ReadMode.ReadThru};
                item = _cache.Get<Customer>(key,readThruOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return item;
        }


        //------------------------------------------------------------------------------------------
        //Simple CRUD Operations as CacheItems


        //Add CacheItem
        public void AddCacheItem(string key, Customer customer)
        {
            try
            {
                var writeThruOptions = new WriteThruOptions {Mode = WriteMode.WriteBehind};

                var cacheItem = new CacheItem(customer);
                Console.WriteLine("Before Adding");

                cacheItemVersion = _cache.Add(key, cacheItem, writeThruOptions);

                cacheItem = _cache.GetCacheItem(key);
                cachedCustomer = cacheItem.GetValue<Customer>();
                Console.WriteLine();
                Console.WriteLine("Customer: " + key + " -- Added");
                Console.WriteLine("Name: " + cachedCustomer.ContactName);
                Console.WriteLine("Company: " + cachedCustomer.CompanyName);
                Console.WriteLine("Address: " + cachedCustomer.Address);
                Console.WriteLine("Item Version: " + cacheItemVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Update CacheItem
        public void UpdateCacheItem(string key, Customer customer)
        {
            try
            {
                var writeThruOptions = new WriteThruOptions {Mode = WriteMode.WriteThru};
                var cacheItem = new CacheItem(customer);
                Console.WriteLine("Before Updating");
                cacheItem = new CacheItem(customer);

                cacheItemVersion = _cache.Insert(key, cacheItem, writeThruOptions);
                cacheItem = _cache.GetCacheItem(key);
                cachedCustomer = cacheItem.GetValue<Customer>();

                Console.WriteLine();
                Console.WriteLine("Customer: " + key + " -- Updated");
                Console.WriteLine("Name: " + cachedCustomer.ContactName);
                Console.WriteLine("Company: " + cachedCustomer.CompanyName);
                Console.WriteLine("Address: " + cachedCustomer.Address);
                Console.WriteLine("Item Version: " + cacheItemVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Remove CacheItem
        public void RemoveCacheItem(string key)
        {
            try
            {
                var writeThruOptions = new WriteThruOptions {Mode = WriteMode.WriteThru};

                Console.WriteLine("Before Removing");
                _cache.Remove(key,null,null,writeThruOptions);
                Console.WriteLine();
                Console.WriteLine("Customer: " + key + " -- Removed");
                cachedCustomer = _cache.Get<Customer>(key);
                if (cachedCustomer == null)
                {
                    Console.WriteLine(key + " doesn't exist");
                }
                else
                {
                    Console.WriteLine(key + " was not removed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Get CacheItem
        public CacheItem GetCacheItem(string key)
        {
            CacheItem item = null;
            try
            {
                var readThruOptions = new ReadThruOptions {Mode = ReadMode.ReadThru};
                item = _cache.GetCacheItem(key,readThruOptions);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return item;
        }


        //----------------------------------------------------------------------------------------------
        //Bulk CRUD Operations


        //Add Bulk Customers
        public void AddBulk(string[] keys, Customer[] customers)
        {
            try
            {
                WriteThruOptions writeThruOptions = new WriteThruOptions(mode:WriteMode.WriteBehind);
                IDictionary<string, CacheItem> dictionary = new Dictionary<string, CacheItem>();
                int count = 0;
                foreach (Customer customer in customers)
                {
                    var cacheItem = new CacheItem(customer);
                    dictionary.Add(keys[count], cacheItem);
                    count++;
                }

                Console.WriteLine("Before Adding");

                IDictionary<string, Exception> keysNotAdded = _cache.AddBulk(dictionary,writeThruOptions);
                var items = _cache.GetBulk<Customer>(keys);
                count = 0;
                foreach (var item in items)
                {
                    Console.WriteLine();
                    Console.WriteLine("Customer: " + item.Key + " -- Added");
                    Console.WriteLine("Name: " + item.Value.ContactName);
                    Console.WriteLine("Company: " + item.Value.CompanyName);
                    
                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Add Bulk Products
        public void AddBulk(string[] Keys, Product[] products)
        {
            try
            {
                Console.WriteLine("Before Adding");

                IDictionary<string, CacheItem> dictionary = new Dictionary<string, CacheItem>();
                int count = 0;
                foreach (Product product in products)
                {
                    var cacheItem = new CacheItem(product);
                    dictionary.Add(Keys[count], cacheItem);
                    count++;
                }

                _cache.AddBulk(dictionary);
                var items = _cache.GetBulk<Product>(Keys);

                count = 0;
                foreach (var item in items)
                {
                    Console.WriteLine();
                    Console.WriteLine("Product: " + Keys[count] + " -- Added");
                    Console.WriteLine("ProductID: " + item.Value.ProductId);
                    Console.WriteLine("UnitPrice: " + item.Value.UnitPrice);
                    Console.WriteLine("Name: " + item.Value.ProductName);

                    count++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Update Bulk
        public void UpdateBulk(string[] keys, Customer[] customers)
        {
            try
            {
                IDictionary<string, CacheItem> dictionary = new Dictionary<string, CacheItem>();
                int count = 0;
                foreach (Customer customer in customers)
                {
                    var cacheItem = new CacheItem(customer);
                    dictionary.Add(keys[count], cacheItem);
                    count++;
                }
                Console.WriteLine();
                Console.WriteLine("Before Updating");
                IDictionary<string, Exception> keysFailedToInsert = _cache.InsertBulk(dictionary);

                var items = _cache.GetBulk<Customer>(keys);
                count = 0;
                foreach (var item in items)
                {
                    Console.WriteLine();
                    Console.WriteLine("Customer: " + item.Key + " -- Updated");
                    Console.WriteLine("Name: " + item.Value.ContactName);
                    Console.WriteLine("Company: " + item.Value.CompanyName);
                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        //Remove Bulk
        public void RemoveBulk(string[] keys)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Before Removing");
                _cache.RemoveBulk(keys);
                Console.WriteLine("Removed!!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Get Bulk
        public List<Customer> GetBulk(string[] keys)
        {
            List<Customer> customerList = new List<Customer>();
            try
            {
                var readThruOptions = new ReadThruOptions {Mode = ReadMode.ReadThru};

                var item = _cache.GetBulk<Customer>(keys,readThruOptions);
                foreach (var entry in item)
                {
                    customerList.Add(entry.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return customerList;
        }


        //-----------------------------------------------------------------------------------------------
        //Group CRUD Operations


        //Add an item with a Group
        public void AddWithGroup(string key, Customer customer, string groupName)
        {
            try
            {
                var cacheItem = new CacheItem(customer) {Group = groupName};
                Console.WriteLine("Before Adding");
                CacheItemVersion version = _cache.Add(key, cacheItem);

                ICollection<string> retrievedKeys = _cache.SearchService.GetGroupKeys(groupName);
                if (retrievedKeys.Count > 0)
                {
                    foreach (string item in retrievedKeys)
                    {
                        Console.WriteLine("Key: " + item + " -- Added!! with group name: " + groupName);
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("There was a problem adding: " + key + " with the provided group: " + groupName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Add Group to Existing Items
        public void AddGroup(string[] keys, string groupName)
        {
            try
            {
                foreach (var item in keys)
                {
                    var cacheItem = _cache.GetCacheItem(item);
                    cacheItem.Group = groupName;
                    _cache.Insert(item, cacheItem);
                    Console.WriteLine("GroupName: " + groupName + " has been added to Keys: " + item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Update Group
        public void UpdateGroup(string currentGroupName, string updatedGroupName)
        {
            try
            {
                ICollection<string> retrievedKeys = _cache.SearchService.GetGroupKeys(currentGroupName);
                foreach (var item in retrievedKeys)
                {
                    var cacheItem = _cache.GetCacheItem(item);
                    cacheItem.Group = updatedGroupName;
                    _cache.Insert(item, cacheItem);
                    Console.WriteLine(item + " updated with GroupName: " + updatedGroupName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Remove Group
        public void RemoveGroup(string[] keys)
        {
            try
            {
                foreach (var item in keys)
                {
                    var cacheItem = _cache.GetCacheItem(item);
                    cacheItem.Group = null;
                    _cache.Insert(item, cacheItem);
                    Console.WriteLine("Group Removed For Key: " + item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Remove Group Data
        public void RemoveGroupData(string groupName)
        {
            try
            {
                _cache.SearchService.RemoveGroupData(groupName);
                Console.WriteLine("Items with GroupName: " + groupName + " have been removed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Get Group Data
        public List<Customer> GetGroupData(string groupName)
        {
            List<Customer> customerList = new List<Customer>();
            try
            {
                var item = _cache.SearchService.GetGroupKeys(groupName);
                foreach (var entry in item)
                {
                    customerList.Add(_cache.Get<Customer>(entry));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return customerList;
        }


        //--------------------------------------------------------------------------------------------------
        //CRUD Operations with Tags


        //Add an item with tags
        public void AddWithTags(string key, Customer customer, Tag[] tags)
        {
            try
            {
                var cacheItem = new CacheItem(customer); ;
                cacheItem.Tags = tags;
                _cache.Add(key, cacheItem);
                Console.WriteLine("Key: " + key + " has been added with the provided tags");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Add Tags to Existing items
        public void AddTags(string[] keys, Tag[] tags)
        {
            foreach (var item in keys)
            {
                var cacheItem = _cache.GetCacheItem(item);
                cacheItem.Tags = tags;
                _cache.Insert(item, cacheItem);
                Console.WriteLine("Provided tags have been added to the Keys: " + item);
            }
        }


        //Replace Tags
        public void ReplaceTags(Tag[] currentTags, Tag[] UpdatedTags)
        {
            try
            {
                ICollection<string> retrievedKeysByAnyTags = _cache.SearchService.GetKeysByTags(currentTags, TagSearchOptions.ByAnyTag);
                foreach (var item in retrievedKeysByAnyTags)
                {
                    var cacheItem = _cache.GetCacheItem(item);
                    cacheItem.Tags = null;
                    cacheItem.Tags = UpdatedTags;
                    _cache.Insert(item, cacheItem);
                    Console.WriteLine("Key: " + item + " updated with new tags");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Append Tags
        public void AppendTags(Tag[] currentTags, Tag[] UpdatedTags)
        {
            try
            {
                List<Tag> tagsList = new List<Tag>();
                foreach (var item in currentTags)
                {
                    tagsList.Add(item);
                }
                foreach (var item in UpdatedTags)
                {
                    tagsList.Add(item);
                }

                ICollection<string> retrievedKeysByAnyTags = _cache.SearchService.GetKeysByTags(currentTags, TagSearchOptions.ByAnyTag);
                
                foreach (var item in retrievedKeysByAnyTags)
                {
                    var cacheItem = _cache.GetCacheItem(item);
                    cacheItem.Tags = null;
                    cacheItem.Tags = tagsList.ToArray();
                    _cache.Insert(item, cacheItem);
                    Console.WriteLine("Key: " + item + " appended with new tags");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Remove Tags
        public void RemoveTags(string[] keys)
        {
            try
            {
                foreach (var item in keys)
                {
                    var cacheItem = _cache.GetCacheItem(item);
                    cacheItem.Tags = null;
                    _cache.Insert(item, cacheItem);
                    Console.WriteLine("Tags Removed from Key: " + item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Remove Tag Data
        public void RemoveTagData(Tag[] tags)
        {
            try
            {
                _cache.SearchService.RemoveByTags(tags, TagSearchOptions.ByAnyTag);
                Console.WriteLine("The provided tags have been removed from all relevant items");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Get Data By Any Tag
        public List<Customer> GetByAnyTag(Tag[] tags)
        {
            List<Customer> customerList = new List<Customer>();
            try
            {
                var item = _cache.SearchService.GetKeysByTags(tags, TagSearchOptions.ByAnyTag);
                foreach (var entry in item)
                {
                    customerList.Add(_cache.Get<Customer>(entry));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return customerList;
        }


        //Get Data By All Tag
        public List<Customer> GetByAllTags(Tag[] tags)
        {
            List<Customer> customerList = new List<Customer>();
            try
            {
                var item = _cache.SearchService.GetKeysByTags(tags, TagSearchOptions.ByAllTags);
                foreach (var entry in item)
                {
                    customerList.Add(_cache.Get<Customer>(entry));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return customerList;
        }


        //---------------------------------------------------------------------------------------------------
        //NamedTags in CRUD


        //Add an item with a Named Tag
        public void AddWithNamedTag(string key, Customer customer, NamedTagsDictionary namedTags)
        {
            try
            {
                CacheItem cacheItem = new CacheItem(customer) {NamedTags = namedTags};
                _cache.Add(key, cacheItem);
                Console.WriteLine("Key: " + key + " has been added with the provided Named Tags");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Add Named Tag to Existing Items
        public void AddNamedTag(string[] keys, NamedTagsDictionary namedTags)
        {
            try
            {
                foreach (var items in keys)
                {
                    var cacheItem = _cache.GetCacheItem(items);
                    cacheItem.NamedTags = namedTags;
                    _cache.Insert(items, cacheItem);
                    Console.WriteLine("The provided Named Tags have been added to the Keys: " + items);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Replace Named Tags
        public void ReplaceNamedTags(string[] keys, NamedTagsDictionary updatedNamedTags)
        {
            try
            {
                foreach (var items in keys)
                {
                    var cacheItem = _cache.GetCacheItem(items);
                    cacheItem.NamedTags = null;
                    cacheItem.NamedTags = updatedNamedTags;
                    _cache.Insert(items, cacheItem);
                    Console.WriteLine("Named Tags of Key: " + items + " have been updated.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Remove Named Tags
        public void RemoveNamedTags(string[] keys)
        {
            try
            {
                foreach (var items in keys)
                {
                    var cacheItem = _cache.GetCacheItem(items);
                    cacheItem.NamedTags = null;
                    _cache.Insert(items, cacheItem);
                    Console.WriteLine("Named Tags of Key: " + items + " have been removed.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Search Items By Named Tags (Sample)
        public void SearchByNamedTags()
        {
            string query = "SELECT NCache.Customer WHERE this.FlashSaleDiscount = ?";
            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("FlashSaleDiscount", 0.5);

            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    Customer result = reader.GetValue<Customer>(1);
                    Console.WriteLine("\nComing through Named Tag Search Query");
                    Console.WriteLine("ContactName: " + result.ContactName);
                    Console.WriteLine("Address: " + result.Address);
                    Console.WriteLine("CompanyName: " + result.CompanyName);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("The result set is empty");
            }
        }



        //-----------------------------------------------------------------------------------------------
        //Data Structures



        //List
        public IDistributedList<Customer> CreateList(string key, Customer[] customers)
        {
            IDistributedList<Customer> myList = _cache.DataTypeManager.CreateList<Customer>(key);

            foreach (var customer in customers)
            {
                myList.Add(customer);
            }

            return myList;
        }


        //Queue
        public IDistributedQueue<Customer> CreateQueue(string key, Customer[] customers)
        {
            IDistributedQueue<Customer> myQueue = _cache.DataTypeManager.CreateQueue<Customer>(key);

            foreach (var customer in customers)
            {
                myQueue.Enqueue(customer);
            }

            return myQueue;
        }


        //HashSets
        public IDistributedHashSet<Customer> CreateHashSet(string key, Customer[] customers)
        {
            IDistributedHashSet<Customer> mySet = _cache.DataTypeManager.CreateHashSet<Customer>(key);
            foreach (var customer in customers)
            {
                mySet.Add(customer);
            }
            return mySet;
        }


        //Dictionaries
        public IDistributedDictionary<string,Customer> CreateDictionaries(string masterKey, Customer[] customers)
        {
            IDistributedDictionary<string,Customer> myDictionary = _cache.DataTypeManager.CreateDictionary<string,Customer>(masterKey);
            foreach (var customer in customers)
            {
                string key = $"Customer:{customer.CustomerId}";
                myDictionary.Add(key,customer);
            }
            return myDictionary;
        }


        //Counter
        public ICounter CreateCounter(string key, long initialValue)
        {
            ICounter counter = _cache.DataTypeManager.CreateCounter(key, initialValue);
            //counter.IncrementBy(2);
            //counter.DecrementBy(2);
            return counter;
        }




        //---------------------------------------------------------------------------------------------------
        //Dependencies




        //Key Dependency
        public void AddKeyDependency(string customerKey, Customer customer, string orderKey, Order order)
        {
            var cacheItem = new CacheItem(customer);
            _cache.Add(customerKey, cacheItem);
            Console.WriteLine(customerKey + " Added!");

            cacheItem = new CacheItem(order) {Dependency = new KeyDependency(customerKey)};

            Console.WriteLine("Order Dependency Attached");
            _cache.Add(orderKey, cacheItem);
            Console.WriteLine(orderKey + " Added!");

            Console.WriteLine("Removing: " + customerKey);
            _cache.Remove(customerKey);
            Console.WriteLine(customerKey + " Removed!");
            Console.WriteLine("Accessing order after removing customer");
            var orderItem = _cache.Get<Order>(orderKey);
            if (orderItem == null)
            {
                Console.WriteLine(orderKey + " doesn't exist");
            }
            else
            {
                Console.WriteLine("Order wasn't deleted");
            }
        }


        //Add Key Dependency to Existing Items
        public void AddDependencyToExistingItems(string independentKey, string dependentKey)
        {
            var dependency = new KeyDependency(independentKey);
            var attr = new CacheItemAttributes {Dependency = dependency};
            _cache.UpdateAttributes(dependentKey, attr);
            Console.WriteLine("Key Based Dependency Added!");
        }


        //File Based Dependency
        public void AddFileBasedDependency(string key, string filePath)
        {
            string TestData = _cache.Get<string>(key);

            if (string.IsNullOrEmpty(TestData))
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    TestData = streamReader.ReadToEnd();
                }
            }

            string filepath = filePath;
            var cacheItem = new CacheItem(TestData)
            {
                Dependency = new FileDependency(filepath, DateTime.Now.AddMinutes(20))
            };
            _cache.Insert(key, cacheItem);
            Console.WriteLine("File Dependency Added");
        }

        //Aggregate Dependencies
        public void AggregateDependencies(string key, string filePath, Order order)
        {
            var cacheItem = new CacheItem(order);
            string TestData = _cache.Get<string>(key);

            if (string.IsNullOrEmpty(TestData))
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    TestData = streamReader.ReadToEnd();
                }
            }

            _cache.Insert(key, TestData);
            var aggregateDependency = new AggregateCacheDependency();
            aggregateDependency.Dependencies.Add(new FileDependency(filePath));
            aggregateDependency.Dependencies.Add(new KeyDependency(key));

            cacheItem.Dependency = aggregateDependency;
            _cache.Insert(key, cacheItem);
            Console.WriteLine("Aggregate Dependency Added!");
        }

        //SQL Dependencies
        public void AddSqlDependencies()
        {
            string query =
                "SELECT CustomerID, Address, City FROM Customers WHERE CustomerID = 'LACOR'";
            string query2 = "SELECT * FROM Orders WHERE CustomerID = @CustomerID";

            SqlCacheDependency sqlDependency = new SqlCacheDependency(connectionString, query);

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(query2, sqlConnection);
                cmd.Parameters.AddWithValue("@CustomerID", "OTTIK");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order orderObject = new Order
                        {
                            OrderId = reader["OrderID"].ToString(),
                            ShipAddress = reader["ShipAddress"].ToString(),
                            ShipCity = reader["ShipCity"].ToString(),
                            ShipCountry = reader["ShipCountry"].ToString(),
                            Freight = reader["Freight"].ToString()
                        };

                        string key = $"Order:{orderObject.OrderId}";
                        CacheItem item = new CacheItem(orderObject) { Dependency = sqlDependency };
                        _cache.Insert(key, item);
                        Console.WriteLine(key + " Added!");
                        Console.WriteLine("Freight: " + orderObject.Freight);
                        Console.WriteLine();
                    }
                }
                sqlConnection.Close();
            }
            Console.WriteLine("SQL Dependency Added!");
        }


        //---------------------------------------------------------------------------------------------------
        //Searching


        //Continuous Query Search by GroupName
        public void ContinuousQuerySearchByGroupName(string groupName)
        {
            string query = "SELECT $VALUE$ FROM Entities.Customer WHERE $Group$ = ?";
            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("$Group$", groupName);
            var cQuery = new ContinuousQuery(queryCommand);

            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemAdded, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemUpdated, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemRemoved, EventDataFilter.DataWithMetadata);

            _cache.MessagingService.RegisterCQ(cQuery);
            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    Customer customer = reader.GetValue<Customer>(1);

                    Console.WriteLine("Inside Continuous Query Block");

                    Console.WriteLine("Name: " + customer.ContactName);
                    Console.WriteLine("Company: " + customer.CompanyName);
                    Console.WriteLine("Address: " + customer.Address);
                }
            }
            else
            {
                Console.WriteLine("Query Result Null");
                return;
            }
        }

        public void ContinuousQuerySearchByTags(string tag)
        {
            string query = "SELECT $VALUE$ FROM Entities.Customer WHERE $Tag$ = ?";
            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("$Tag$", tag);
            var cQuery = new ContinuousQuery(queryCommand);

            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemAdded, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemUpdated, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemRemoved, EventDataFilter.DataWithMetadata);

            _cache.MessagingService.RegisterCQ(cQuery);
            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    Customer customer = reader.GetValue<Customer>(1);

                    Console.WriteLine("Inside Continuous Query Block");

                    Console.WriteLine("Name: " + customer.ContactName);
                    Console.WriteLine("Company: " + customer.CompanyName);
                    Console.WriteLine("Address: " + customer.Address);
                }
            }
            else
            {
                Console.WriteLine("Query Result Null");
                return;
            }
        }

        public void ContinuousQuerySearchByNamedTags(string namedTagKey, string namedTagValue)
        {
            string query = $"SELECT $VALUE$ FROM Entities.Customer WHERE {namedTagKey} = ?";
            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add(namedTagKey, namedTagValue);
            var cQuery = new ContinuousQuery(queryCommand);

            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemAdded, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemUpdated, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemRemoved, EventDataFilter.DataWithMetadata);

            _cache.MessagingService.RegisterCQ(cQuery);
            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    Customer customer = reader.GetValue<Customer>(1);

                    Console.WriteLine("Inside Continuous Query Block");

                    Console.WriteLine("Name: " + customer.ContactName);
                    Console.WriteLine("Company: " + customer.CompanyName);
                    Console.WriteLine("Address: " + customer.Address);
                }
            }
            else
            {
                Console.WriteLine("Query Result Null");
                return;
            }
        }

        public void ContinuousQueryForProductUnitPrice(int lowerLimitPrice, int upperLimitPrice)
        {
            string query = "SELECT $VALUE$ FROM Entities.Product WHERE UnitPrice > ? AND UnitPrice < ?";
            var queryCommand = new QueryCommand(query);
            ArrayList myArrayList = new ArrayList()
            {
                lowerLimitPrice,upperLimitPrice
            };
            queryCommand.Parameters.Add("UnitPrice", myArrayList);
            //queryCommand.Parameters.Add("UnitPrice", upperLimitPrice);
            var cQuery = new ContinuousQuery(queryCommand);

            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemAdded, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemUpdated, EventDataFilter.DataWithMetadata);
            cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallback), EventType.ItemRemoved, EventDataFilter.DataWithMetadata);
            _cache.MessagingService.RegisterCQ(cQuery);
            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    Product product = reader.GetValue<Product>(1);
                    Console.WriteLine("Inside Continuous Query Block");
                    Console.WriteLine("ProductID: " + product.ProductId);
                    Console.WriteLine("Name: " + product.ProductName);
                    Console.WriteLine("UnitPrice: " + product.UnitPrice);
                }
            }
            else
            {
                Console.WriteLine("Query Result Null");
                return;
            }
        }

        //----------------------------------------------------------------------------------------------------
        //Locking


        //-------------------
        //Pessimistic Locking


        //Lock Item in Cache
        public void PessimisticLocking(string key)
        {
            LockHandle lockHandle;
            TimeSpan lockSpan = TimeSpan.FromMinutes(10);
            bool LockAcquired = _cache.Lock(key, lockSpan, out lockHandle);
            if (LockAcquired)
            {
                Console.WriteLine("\n" + key + " Locked\n");
            }
            else
            {
                Console.WriteLine("\n" + key + " couldn't be locked\n");
            }
        }


        //Unlock Item in Cache
        public void PessimisticUnlocking(string key)
        {
            bool acquireLock = true;
            TimeSpan lockSpan = TimeSpan.FromSeconds(10);
            LockHandle lockHandle = null;
            _cache.Get<Customer>(key, acquireLock, lockSpan, ref lockHandle);
               
            if (lockHandle != null)
            {
                Console.WriteLine("\nLock acquired\n");
                _cache.Unlock(key, lockHandle);
                Console.WriteLine(key + " unlocked!");
            }
            else
            {
                Console.WriteLine("\nLock couldn't be acquired\n");
            }

        }


        //------------------
        //Optimistic Locking

        //Update Item in Cache
        public void UpdateItem_OptimisticLocking(string key)
        {
            CacheItemVersion itemVersion = null;
            try
            {
                var cacheItem = _cache.GetCacheItem(key, ref itemVersion);
                if (cacheItem != null)
                {
                    var customer = new Customer();
                    customer = cacheItem.GetValue<Customer>();
                    customer.Address = "Updated Address";
                    var updatedItem = new CacheItem(customer) {Version = itemVersion};
                    _cache.Insert(key, cacheItem);
                    Console.WriteLine("Item updated with optimistic locking involved");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        //Get Item in Cache
        public Customer GetItem_OptimisticLocking(string key, CacheItemVersion itemVersion)
        {
            Customer result = null;
            try
            {
                result = _cache.GetIfNewer<Customer>(key, ref itemVersion);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        //Remove Item in Cache
        public void RemoveItem_OptimisticLocking(string key, CacheItemVersion itemVersion)
        {
            try
            {
                _cache.Remove(key, null,itemVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //Publish Subscribe Messaging
        

        //Create Topic
        public void CreateTopic(string topicName)
        {
            _cache.MessagingService.CreateTopic(topicName);
            Console.WriteLine("Topic: " + topicName + " Created!");
        }


        //Delete Topic
        public void DeleteTopic(string topicName)
        {
            _cache.MessagingService.DeleteTopic(topicName);
            Console.WriteLine("Topic: " + topicName + " Deleted!");
        }


        //Publish Messages to Topics
        public void PublicMessagesToTopics(string topicName, Customer customer)
        {
            try
            {
                //Create Topic
                ITopic topic = _cache.MessagingService.GetTopic(topicName);

                //Delete Topic
                //cache.MessagingService.DeleteTopic(topicName);

                //Delete task Asynchronously
                //Task task = cache.MessagingService.DeleteTopicAsync(topicName);

                if (topic != null)
                {
                    var orderMessage = new Message(customer) { ExpirationTime = TimeSpan.FromMinutes(20) };
                    topic.MessageDeliveryFailure += OnFailureMessageReceived;
                    topic.OnTopicDeleted = TopicDeleted;
                    topic.Publish(orderMessage, DeliveryOption.All, true);
                    Console.WriteLine("Publisher successfully published the messages to the topic: " + topicName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        //Subscribe for Topic Messages
        public void SubscribeForTopicMessages(string topicName)
        {
            try
            {
                ITopic topic = _cache.MessagingService.GetTopic(topicName);
                if (topic != null)
                {
                    ITopicSubscription ordSubscribe = topic.CreateSubscription(MessageReceived);
                    Console.WriteLine("Subscriber successfully subscribed to the topic: " + topicName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        //Durable Subscription
        public void DurableSubscription(string topicName, string subscriptionName)
        {
            ITopic topic = _cache.MessagingService.GetTopic(topicName);

            IDurableTopicSubscription ordSubscriber = topic.CreateDurableSubscription(subscriptionName,
                SubscriptionPolicy.Shared, MessageReceived, TimeSpan.FromMinutes(2000));
            Console.WriteLine("Durable Subscription Added!");
        }


        //Pattern Based Subscription
        public void PatternBasedSubscription(string topicName)
        {
            ITopic topic = _cache.MessagingService.GetTopic(topicName, TopicSearchOptions.ByPattern);
            ITopicSubscription ordSubscriber = topic.CreateSubscription(MessageReceived);
            Console.WriteLine("Pattern Based Subscription Added!");
        }


        //Pattern Based Durable Subscription
        public void PatternBasedDurableSubscription(string topicName, string subscriptionName)
        {
            ITopic topic = _cache.MessagingService.GetTopic(topicName, TopicSearchOptions.ByPattern);
            IDurableTopicSubscription ordSubscriber = topic.CreateDurableSubscription(subscriptionName,
                SubscriptionPolicy.Exclusive, MessageReceived, TimeSpan.FromMinutes(2000));
            Console.WriteLine("Pattern Based Durable Subscription Added!");
        }


        //Pattern Based Subscription with Failure Notification
        public void PatternBasedSubscriptionWithFailureNotification(string topicName)
        {
            try
            {
                ITopic topic = _cache.MessagingService.GetTopic(topicName, TopicSearchOptions.ByPattern);
                if (topic != null)
                {
                    topic.MessageDeliveryFailure += OnMessageDeliveryFailure;
                    Console.WriteLine("Pattern Based Subscription with Failure Notification Added!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //----------------------------------------------------------------------------------------------------
        //Pub Sub Callbacks


        //Message Receive Failure
        private void OnFailureMessageReceived(object sender, MessageFailedEventArgs args)
        {
            Console.WriteLine("Pub Sub Event Trigger -- Message delivery failed.");
        }

        //Topic Deleted
        public void TopicDeleted(object sender, TopicDeleteEventArgs args)
        {
            Console.WriteLine("Pub Sub Event Trigger -- Topic Deleted.");
        }

        //Message Received
        private void MessageReceived(object sender, MessageEventArgs args)
        {
            if (args.Message.Payload is Customer customer)
            {
                Console.WriteLine("Pub Sub Event Trigger -- Message Received.");
                Console.WriteLine("Orders details are as follows: ");
                Console.WriteLine("\nContactName: " + customer.ContactName);
                Console.WriteLine("\nCompanyName: " + customer.CompanyName);
                Console.WriteLine("\nAddress: " + customer.Address);
                Console.WriteLine("\nContactTitle: " + customer.ContactTitle);
            }
            else
            {
                Console.WriteLine("Empty Message Received");
            }
        }

        //Message Delivery Failure
        private void OnMessageDeliveryFailure(object sender, MessageFailedEventArgs args)
        {
            Console.WriteLine("Pub Sub Event Trigger ---- Message Delivery Failure");
        }




        //----------------------------------------------------------------------------------------------------
        //Item Level Callbacks


        //On Cache Data Modification
        private void OnCacheDataModification(string key, CacheEventArg args)
        {
            switch (args.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine("\nCache Level Event Trigger: Item Added to Cache\n");
                    break;
                case EventType.ItemUpdated:
                    Console.WriteLine("\nCache Level Event Trigger: Item Updated in Cache\n");
                    break;
                case EventType.ItemRemoved:
                    Console.WriteLine("\nCache Level Event Trigger: Item Removed from Cache\n");
                    break;
            }
        }


        //----------------------------------------------------------------------------------------------------
        //Management Level Callbacks


        //Cache Cleared Event
        private void OnCacheCleared()
        {
            Console.WriteLine("\nCache Cleared -- Event Trigger!!\n");
        }


        //Cache Stopped Event
        private void OnCacheStopped(string cacheName)
        {
            Console.WriteLine("\n" + cacheName + " Stopped -- Event Triggered!!\n");
        }


        //Member Joined
        private void OnMemberJoined(NodeInfo nodeInfo)
        {
            Console.Write("Cache Level Event Trigger: A Member Joined with Ip: " + nodeInfo.IpAddress + " and port no: " + nodeInfo.Port);
        }


        //Member Left
        private void OnMemberLeft(NodeInfo nodeInfo)
        {
            Console.Write("Cache Level Event Trigger: A Member Left with Ip: " + nodeInfo.IpAddress + " and port no: " + nodeInfo.Port);
        }




        //----------------------------------------------------------------------------------------------------
        //Indexed Searching Callbacks


        //Query Item Callback
        private void QueryItemCallback(string key, CQEventArg arg)
        {
            switch (arg.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine("Continuous Query Event Trigger --- Item Added");
                    break;

                case EventType.ItemUpdated:
                    Console.WriteLine("Continuous Query Event Trigger --- Item Updated");
                    break;

                case EventType.ItemRemoved:
                    Console.WriteLine("Continuous Query Event Trigger --- Item Removed");
                    break;
            }
        }





        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //---------------------------------Server Side Features-----------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------




        //Single Entry Processor
        public void EntryProcessor(string key, Customer customer)
        {
            IDictionary<string, CacheItem> dictionary = new Dictionary<string, CacheItem>();
            var cacheItem = new CacheItem(customer);
            dictionary.Add(key, cacheItem);

            _cache.Insert(key, cacheItem);
            Customer returnedValue = (Customer)_cache.ExecutionService.Invoke(key, customEntryProcessor);
            Console.WriteLine("Through Entity Processor");
            Console.WriteLine("ContactName: " + returnedValue.ContactName);
            Console.WriteLine("CompanyName: " + returnedValue.CompanyName);
            Console.WriteLine("Address: " + returnedValue.Address);

            var returnedEntries = (Customer)_cache.ExecutionService.Invoke(key, customEntryProcessor);

            Console.WriteLine("Returned Entry: " + customer.CustomerId);

        }


        //Bulk Entries Processor
        public void EntryProcessorBulk(string[] keys, Customer[] customers)
        {
            IDictionary<string, CacheItem> dictionary = new Dictionary<string, CacheItem>();
            int count = 0;
            foreach (Customer customer in customers)
            {
                var cacheItem = new CacheItem(customer);
                dictionary.Add(keys[count], cacheItem);
                count++;
            }

            _cache.InsertBulk(dictionary);
            Customer returnedValue = (Customer) _cache.ExecutionService.Invoke("Customer:GHOST", customEntryProcessor);
            Console.WriteLine("Through Entity Processor");
            Console.WriteLine("ContactName: " + returnedValue.ContactName);
            Console.WriteLine("CompanyName: " + returnedValue.CompanyName);
            Console.WriteLine("Address: " + returnedValue.Address);

            ICollection returnedEntries = _cache.ExecutionService.Invoke(keys, customEntryProcessor);

            foreach (IEntryProcessorResult returnedEntry in returnedEntries)
            {
                if (returnedEntry.IsSuccessful)
                {
                    var result = (Customer)returnedEntry.Value;
                    Console.WriteLine("Returned Entry: " + result.CustomerId);
                }
            }
        }

        
        //Aggregate Functions
        public void TestAggregate()
        {
            object value;
            try
            {
                value =  _cache.ExecutionService.Aggregate(new ValueExtractor(), new AggregateClass("COUNT"));
                value =  _cache.ExecutionService.Aggregate(new ValueExtractor(), BuiltInAggregator.Count());
                Console.WriteLine("Aggregate Count: " + value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Extensible Dependency
        public void TestExtensibleDependency(string customerID)
        {
            extensibleDependency = new ExtensibleDependencyProvider(customerID, connectionString);
            if (extensibleDependency.Initialize())
            {
                Console.WriteLine("Has not Changed");
            }
            else
            {
                Console.WriteLine("Has Changed");
            }
        }

        //Map Reduce

        public void MapReduceExecutor()
        {
            try
            {
                mapReduceTask.Mapper = mapper;
                mapReduceTask.Combiner = new CombinerFactory();
                mapReduceTask.Reducer = new ReducerFactory();

                ITrackableTask trackableTask = _cache.ExecutionService.ExecuteTask(mapReduceTask, keyFilter);

                var item = trackableTask.TaskStatus;
                Console.WriteLine(item.Progress.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }




        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //---------------------------------Session States-----------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------


        //Get Session Data
        public void GetSessionData()
        {
            var allSessionData = _cache.SearchService.GetByTag<object>(new Tag("NC_ASP.net_session_data"));
            foreach (var key in allSessionData)
            {
                Console.WriteLine();
                Console.WriteLine("Session Item Key: " + key.Key);
                Console.WriteLine("Session Item Value: " + key.Value);
            }
        }

        public void GetSessionDataForUserInfo()
        {
            var allSessionData = _cache.SearchService.GetByTag<UserInfo>(new Tag("NC_ASP.net_session_data"));
            foreach (var key in allSessionData)
            {
                Console.WriteLine();
                Console.WriteLine("Session Item Key: " + key.Key);
                Console.WriteLine("Session Item Value: " + key.Value);
            }
        }

        //Get OutputCache Data
        public void GetOutputCacheData()
        {
            var allSessionData = _cache.SearchService.GetKeysByTags(new Tag[1]{ new Tag("NC_ASP.net_output_data")},TagSearchOptions.ByAllTags);
            foreach (var key in allSessionData)
            {
                Console.WriteLine();
                Console.WriteLine("Output Cache Item Key: " + key);
            }
        }

        //Get ViewState Data
        public void GetViewStateData()
        {
            IDictionary<string, object> allViewStateData = _cache.SearchService.GetByTag<object>(new Tag("NC_ASP.net_viewstate_data"));
            foreach (var item in allViewStateData)
            {
                Console.WriteLine();
                Console.WriteLine("View State Item Key: " + item.Key);
                Console.WriteLine("View State Item Value: " + item.Value);
            }
        }
    }
}
