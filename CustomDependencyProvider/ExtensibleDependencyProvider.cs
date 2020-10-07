using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.Dependencies;

using  Alachisoft.NCache.Runtime.DatasourceProviders;
using Alachisoft.NCache.Runtime;
using Alachisoft.NCache.Runtime.Exceptions;


namespace CustomDependencyProvider
{
    public class ExtensibleDependencyProvider: ExtensibleDependency
    {
        private string ConnectionString;
        private string customerID;

        public override bool HasChanged { get; }

        public ExtensibleDependencyProvider(string customerID, string connectionString)
        {
            ConnectionString = connectionString;
            this.customerID = customerID;
        }

        public override bool Initialize()
        {
            return CheckExistance(customerID);
        }

        public bool CheckExistance(string customerID)
        {
            List<String> CustomersIDList = new List<string>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();
                    string queryString = "Select CustomerID from Customers";

                    SqlCommand cmd = new SqlCommand(queryString, sqlConnection);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            CustomersIDList.Add(rdr["CustomerID"].ToString());
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.Write(ex.Message);
            }

            if (CustomersIDList.Contains(customerID))
            {
                return true;
            }
            return false;
        }

    }
}
