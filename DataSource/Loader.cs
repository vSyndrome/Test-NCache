using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Alachisoft.NCache.Runtime.CacheLoader;
using Alachisoft.NCache.Runtime.Caching;
using Entities;

namespace DataSource
{
    public class Loader : ICacheLoader
    {
        private static string connectionString;

        public void Init(IDictionary parameters, string cacheId)
        {
            connectionString = parameters["ConnectionString"].ToString();
        }

        public LoaderResult LoadNext(object userContext)
        {
            LoaderResult loaderResult = new LoaderResult();
            string queryString = "SELECT * FROM Customers";
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand(queryString, sqlConnection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customer customerObject = new Customer
                            {
                                CompanyName = reader["CompanyName"].ToString(),
                                ContactName = reader["ContactName"].ToString(),
                                ContactTitle = reader["ContactTitle"].ToString(),
                                Address = reader["Address"].ToString(),
                                CustomerId = reader["CustomerID"].ToString()
                            };

                            ProviderCacheItem cacheItem = new ProviderCacheItem(customerObject);
                            cacheItem.ResyncOptions.ResyncOnExpiration = true;
                            var key = $"Customer:{customerObject.CustomerId}";
                            var keyValuePair = new KeyValuePair<string, ProviderItemBase>(key, cacheItem);
                            loaderResult.Data.Add(keyValuePair);
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            loaderResult.HasMoreData = false;
            return loaderResult;
        }

        public void Dispose()
        {

        }
    }
}

