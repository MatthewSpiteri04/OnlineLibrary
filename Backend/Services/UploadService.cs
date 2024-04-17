using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Backend.Services;


namespace Backend.Services
{
    public class UploadService : DatabaseConnection
    {
        public UploadService() : base()
        {

        }
        public bool CanUserUpload(int userId)
        {
            int userIsAllowed = -1;

            query = @"DECLARE @Result AS INT = -1;
                      IF EXISTS (
                      SELECT 1 FROM Users U
                      INNER JOIN Roles R ON R.Id = U.RoleId
                      INNER JOIN RolesToPrivileges RP ON R.Id = RP.RoleId
                      INNER JOIN Privileges P ON P.Id = RP.PrivilegeId
                      WHERE U.Id = " + userId +@" AND P.[Description] LIKE 'Academic User'
                      )
	                      SET @Result = 1;
                      ELSE
	                      SET @Result = 0;

                      SELECT @Result";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                userIsAllowed = reader.GetInt32(0);
            }
            reader.Close();

            if (userIsAllowed == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
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

            query = @"INSERT INTO Documents VALUES (" + request.CategoryId + @", '" + request.Title + @"', '" + request.LanguageId + @"', GETDATE(), " + var + @", '" + request.DocumentLocation + @"');";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
            }
            reader.Close();
        }
    }
}
