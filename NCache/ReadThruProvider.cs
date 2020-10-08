using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.DatasourceProviders;

using Entities;

namespace NCache
{
    class ReadThruProvider: IReadThruProvider
    {
        private string connectionString;

        public void Init(IDictionary parameters, string cacheId)
        {
            connectionString = parameters["ConnectionString"].ToString();
        }

        public ProviderCacheItem LoadFromSource(string key)
        {
            Customer value = LoadFromDataSource(key);
            var cacheItem = new ProviderCacheItem(value);
            cacheItem.ResyncOptions.ResyncOnExpiration = true;
            return cacheItem;
        }

        public IDictionary<string, ProviderCacheItem> LoadFromSource(ICollection<string> keys)
        {
            var dictionary = new Dictionary<string, ProviderCacheItem>();
            try
            {
                foreach (string key in keys)
                {
                    ProviderCacheItem cacheItem = new ProviderCacheItem(LoadFromDataSource(key));
                    cacheItem.ResyncOptions.ResyncOnExpiration = true;
                    dictionary.Add(key, cacheItem);
                }
                return dictionary;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            return dictionary;
        }

        public ProviderDataTypeItem<IEnumerable> LoadDataTypeFromSource(string key, DistributedDataType dataType)
        {
            IEnumerable value = null;
            ProviderDataTypeItem<IEnumerable> dataTypeItem = null;

            switch (dataType)
            {
                case DistributedDataType.List:
                    value = new List<object>()
                    {
                        LoadFromDataSource(key)
                    };
                    dataTypeItem = new ProviderDataTypeItem<IEnumerable>(value);
                    break;

                case DistributedDataType.Dictionary:
                    value = new Dictionary<string, object>()
                    {
                        { key ,  LoadFromDataSource(key) }
                    };
                    dataTypeItem = new ProviderDataTypeItem<IEnumerable>(value);
                    break;

                case DistributedDataType.Counter:
                    dataTypeItem = new ProviderDataTypeItem<IEnumerable>(1000);
                    break;
            }

            return dataTypeItem;
        }

        public void Dispose()
        {

        }

        private Customer LoadFromDataSource(string key)
        {
            Customer retrievedObject = null;

            //Implementation

            string queryString = "SELECT * FROM Customers WHERE CustomerID = @CustomerID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if(connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand cmd = new SqlCommand(queryString, connection);
                cmd.Parameters.AddWithValue("@CustomerID", key);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Customer customerObject = new Customer();
                        customerObject.CustomerId = rdr["CustomerID"].ToString();
                        customerObject.CompanyName = rdr["CompanyName"].ToString();
                        customerObject.ContactName = rdr["ContactName"].ToString();
                        customerObject.ContactTitle = rdr["ContactTitle"].ToString();
                        customerObject.Address = rdr["Address"].ToString();
                        retrievedObject = customerObject;
                    }
                }
                connection.Close();
            }
            return retrievedObject;
        }

    }
}
