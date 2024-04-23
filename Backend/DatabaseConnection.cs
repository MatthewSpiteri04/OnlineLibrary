using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Backend
{
    public class DatabaseConnection
    {
        protected SqlConnection conn;
        protected string query;

        //setting up the connection
        public DatabaseConnection()
        {
            conn = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=OnlineLibrary;Integrated Security=True;Encrypt=False;");
            query = "";
        }

        //executing a query
        protected SqlDataReader executeQuery()
        {
            //conn.Open();
            if (query == "")
            {
                throw new Exception("No query has been provided");
            }
            else
            {
                conn.Open();
                SqlDataReader reader = new SqlCommand(query, conn).ExecuteReader();
                return reader;
            }
        }



        protected void executeCommand()
        {
            if (query == "")
            {
                throw new Exception("No query has been provided");

            }

            else
            {
                conn.Open();
                new SqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}