using System;
using System.Data.SqlClient;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Backend.Services;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;


namespace Backend.Services
{
	public class SecurityService : DatabaseConnection
	{
        public SecurityService() : base()
        {

        }
        public User updateUserInfo(UpdateRequest request)
        {
            query = @"UPDATE Users
                      SET [FirstName] = '" + request.FirstName + @"', [LastName] = '" + request.LastName + @"', [Username] = '" + request.Username + @"'
                      WHERE Users.Id =" + request.Id + @";

                      SELECT * FROM Users WHERE [Id] = " + request.Id;

            SqlDataReader reader = executeQuery();

            User user = new User();

            while (reader.Read())
            {
                user = new User() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), Username = reader.GetString(3), Email = reader.GetString(4), Password = reader.GetString(5), RoleId = reader.GetInt32(6) };
            }
            reader.Close();
            conn.Close();

            if (user.Id <= 0)
            {
                return null;
            }

            return user;
        }
        public User updateUserPassword(UpdateRequest request)
        {
            MD5 hasher = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(request.Password);
            byte[] hashBytes = hasher.ComputeHash(inputBytes);

            string passwordHash = Convert.ToHexString(hashBytes);
            query = @"UPDATE Users
                      SET [Password] = '" + passwordHash + @"'
                      WHERE Users.Id =" + request.Id + @";

                      SELECT * FROM Users WHERE [Id] = " + request.Id;

            SqlDataReader reader = executeQuery();

            User user = new User();

            while (reader.Read())
            {
                user = new User() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), Username = reader.GetString(3), Email = reader.GetString(4), Password = reader.GetString(5), RoleId = reader.GetInt32(6) };
            }
            reader.Close();
            conn.Close();

            if (user.Id <= 0)
            {
                return null;
            }

            return user;
        }

        public User searchForFileHandler(int id)
        {
            query = @"SELECT TOP(1) U.* FROM Users U
                    LEFT JOIN  Roles R
                    ON U.RoleId = R.Id
                    LEFT JOIN RolesToPrivileges RP
                    ON R.Id = RP.RoleId
                    LEFT JOIN Privileges P
                    ON P.Id = RP.PrivilegeId
                    WHERE P.[Description] = 'Handle No ID Documents'  AND U.Id != " + id;
            ;

            User user = new User();
            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                user = new User() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), Username = reader.GetString(3), Email = reader.GetString(4), Password = reader.GetString(5), RoleId = reader.GetInt32(6) };
            }
            reader.Close();
            conn.Close();

            return user;
        }

        public void updateDocumentsAndDeleteUser(User headmaster, int id)
        {
            query = @"UPDATE Documents
                    SET [UserId] = " + headmaster.Id + @"
                    WHERE [UserId] = " + id + @";

                    DELETE FROM Users WHERE [Id] = " + id;

            executeCommand();
        }

        public bool getUserDocuments(int id)
        {
            int documentCount = 0;

            query = @"SELECT COUNT(*) FROM Documents WHERE UserId = " + id;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                documentCount = reader.GetInt32(0);
            }
            reader.Close();
            conn.Close();

            if(documentCount <= 0)
            {
                return false;
            } 
            else
            {
                return true;
            }
        }

        public int deleteUser(int id)
        {
            query = @"DELETE FROM Users WHERE [Id] = " + id;
            return executeCommand();
        }

        public void deleteUserAndDocuments(int id)
        {
            DocumentsService _documentsService = new DocumentsService();
            List<Documents> myUploads = _documentsService.getMyUploads(id);

            foreach (Documents document in myUploads)
            {
                query = @"DELETE FROM DocumentAttributes WHERE DocumentId = " + document.Id + @";
                          DELETE FROM Favourites WHERE DocumentId = " + document.Id + @";
                          DELETE FROM Documents WHERE Id = " + document.Id;

                executeCommand();
            }

            query = @"DELETE FROM Favourites WHERE UserId = " + id + @";
                      DELETE FROM Users WHERE [Id] = " + id;


            executeCommand();
        }
    }
}
