using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public class UploadService : DatabaseConnection
    {
        public UploadService() : base()
        {

        }

        public void SaveUploadedFile(UploadDatabaseRequest request)
        {
            int var = -1;
            if (request.PublicAccess == true)
            {
                var = 1;
            }
            else
            {
                var = 0;
            }

            query = @"INSERT INTO Documents VALUES (" + request.CategoryId + @", '" + request.Title + @"', '" + request.Language + @"', GETDATE(), " + var + @", '" + request.DocumentLocation + @"');";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
            }
        }
    }
}
