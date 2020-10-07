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
    class WriteThruProvider : IWriteThruProvider
    {
        private SqlConnection sqlConnection;
        private string _connectionString;

        public void Init(IDictionary parameters, string cacheId)
        {
            _connectionString = parameters["ConnectionString"].ToString();
            sqlConnection = new SqlConnection(_connectionString);
        }

        public OperationResult WriteToDataSource(WriteOperation operation)
        {
            ProviderCacheItem cacheItem = operation.ProviderItem;
            var customer = cacheItem.GetValue<Customer>();

            switch (operation.OperationType)
            {
                case WriteOperationType.Add:
                    Add(customer);
                    break;
                case WriteOperationType.Delete:
                    Delete(customer);
                    break;
                case WriteOperationType.Update:
                    Update(customer);   
                    break;
            }
            return new OperationResult(operation, OperationResult.Status.Success);
        }

        public ICollection<OperationResult> WriteToDataSource(ICollection<WriteOperation> operations)
        {
            var operationResult = new List<OperationResult>();

            foreach (WriteOperation operation in operations)
            {
                ProviderCacheItem cacheItem = operation.ProviderItem;
                Customer customer = cacheItem.GetValue<Customer>();

                switch (operation.OperationType)
                {
                    case WriteOperationType.Add:
                        Add(customer);
                        break;
                    case WriteOperationType.Delete:
                        Delete(customer);
                        break;
                    case WriteOperationType.Update:
                        Update(customer);
                        break;
                }

                operationResult.Add(new OperationResult(operation, OperationResult.Status.Success));
            }
            return operationResult;
        }

        public ICollection<OperationResult> WriteToDataSource(ICollection<DataTypeWriteOperation> operations)
        {
            var operationResult = new List<OperationResult>();

            foreach (DataTypeWriteOperation operation in operations)
            {
                IList list = new List<Customer>();
                ProviderDataTypeItem<object> cacheItem = operation.ProviderItem;
                Customer customer = (Customer)cacheItem.Data;

                switch (operation.OperationType)
                {
                    case DatastructureOperationType.CreateDataType:
                        list.Add(customer.CustomerId);
                        break;

                    case DatastructureOperationType.AddToDataType:
                        list.Add(customer);
                        break;

                    case DatastructureOperationType.DeleteFromDataType:
                        list.Remove(customer);
                        break;

                    case DatastructureOperationType.UpdateDataType:
                        list.Insert(0, customer);
                        break;
                }
                operationResult.Add(new OperationResult(operation, OperationResult.Status.Success));
            }

            return operationResult;
        }

        public void Dispose()
        {
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }
        }

        private void Add(Customer customer)
        {
            try
            {
                string query =
                    "INSERT INTO Customers(CustomerID, CompanyName, ContactName, ContactTitle, [Address], City, Region, PostalCode, Country, Phone, Fax) VALUES(@CustomerID, @CompanyName, @ContactName, @ContactTitle, @Address, @City, @Region, @PostalCode, @Country, @Phone, @Fax)";
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
                sqlCommand.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
                sqlCommand.Parameters.AddWithValue("@ContactName", customer.ContactName);
                sqlCommand.Parameters.AddWithValue("@ContactTitle", customer.ContactTitle);
                sqlCommand.Parameters.AddWithValue("@Address", customer.Address);
                sqlCommand.Parameters.AddWithValue("@City", "");
                sqlCommand.Parameters.AddWithValue("@Region", "");
                sqlCommand.Parameters.AddWithValue("@PostalCode", "");
                sqlCommand.Parameters.AddWithValue("@Country", "");
                sqlCommand.Parameters.AddWithValue("@Phone", "");
                sqlCommand.Parameters.AddWithValue("@Fax", "");
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Update(Customer customer)
        {
            try
            {
                string query =
                    "UPDATE Customers SET CompanyName = @CompanyName, ContactName = @ContactName,ContactTitle = @ContactTitle,[Address] = @Address WHERE CustomerID = @CustomerID";
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@CustomerID",customer.CustomerId);
                sqlCommand.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
                sqlCommand.Parameters.AddWithValue("@ContactName", customer.ContactName);
                sqlCommand.Parameters.AddWithValue("@ContactTitle", customer.ContactTitle);
                sqlCommand.Parameters.AddWithValue("@Address", customer.Address);
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Delete(Customer customer)
        {
            try
            {
                string query =
                    "DELETE FROM Customers WHERE CustomerID = @CustomerID";
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
