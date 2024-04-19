using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Backend
{
    public class DatabaseConnection
    {
        private SqlConnection conn;
        protected string query;

        //setting up the connection
        public DatabaseConnection()
        {
            conn = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=OnlineLibrary;Integrated Security=True;Encrypt=False;");
            query = "";

            try
            {
                conn.Open();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
        }

        //executing a query
        protected SqlDataReader executeQuery(string query)
        {
            if (query == "")
            {
                new Exception("No query has been provided");
                return null;
            }
            else return new SqlCommand(query, conn).ExecuteReader();
        }
    }
}