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
            conn.Close();

            if (userIsAllowed == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int SaveUploadedFile(UploadDatabaseRequest request)
        {
            int documentId = -1;
            int var = -1;
            if (request.PublicAccess == true)
            {
                var = 1;
            }
            else
            {
                var = 0;
            }

            query = @"INSERT INTO Documents VALUES (" + request.UserId + @", " + request.CategoryId + @", '" + request.Title + @"', '" + request.LanguageId + @"', GETDATE(), " + var + @", '" + request.DocumentLocation + @"' , '" + request.FileExtension + @"');
                      SELECT CAST(SCOPE_IDENTITY() AS INT);";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                documentId = reader.GetInt32(0); 
            }
            reader.Close();
            conn.Close();
            return documentId;
        }

        public void setDocumentAttribute(int documentId, List<AttributeUploadRequest> attributes)
        {
            foreach (AttributeUploadRequest attr in attributes)
            {
                query = @"INSERT INTO DocumentAttributes ([DocumentId], [AttributeId], [Value]) VALUES (" + documentId + @", " + attr.Id + @", '" + attr.Value + @"');";

                executeCommand();
            }
        }

        public List<AttributesTypeRequest> getAttributes(int categoryId)
        {
            List<AttributesTypeRequest> attributes = new List<AttributesTypeRequest>();

            query = @"SELECT A.[Id], A.[Name], AT.[TagName] FROM Categories C
                      INNER JOIN CategoryAttributes CA ON C.Id = CA.CategoryId
                      INNER JOIN Attributes A ON A.Id = CA.AttributeId
                      INNER JOIN AttributeTypes [AT] ON AT.Id = A.TypeId
                      WHERE C.Id = " + categoryId;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                attributes.Add(new AttributesTypeRequest()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Type = reader.GetString(2)
                });
            }
            reader.Close();
            conn.Close();

            return attributes;
        }
    }
}
