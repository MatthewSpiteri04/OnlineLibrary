﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;

namespace Backend.Services
{
    public class UserService : DatabaseConnection
    {
        public UserService() : base()
        {

        }

        public  List<string> getRolePrivileges(int id)
        {
            List<string> user_permissions = new List<string>();

            query = @"SELECT P.Description FROM Users U
                      LEFT JOIN  Roles R
                      ON U.RoleId = R.Id
                      LEFT JOIN RolesToPrivileges RP
                      ON R.Id = RP.RoleId
                      LEFT JOIN Privileges P
                      ON P.Id = RP.PrivilegeId
                      WHERE U.Id = " + id;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    user_permissions.Add(reader.GetString(0));
                }
            }
            reader.Close();
            conn.Close();

            return user_permissions;
        }

        public bool doesUserExist(string login)
        {
            query = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM Users WHERE [Username] = '" + login + "' OR [Email] = '" + login + "') THEN 1 ELSE 0 END;";
            SqlDataReader reader = executeQuery();

            int flag = -1;

            while (reader.Read())
            {
               flag = reader.GetInt32(0);
            }
            reader.Close();
            conn.Close();

            if (flag == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public User createUser(User user)
        {
            MD5 hasher = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(user.Password);
            byte[] hashBytes = hasher.ComputeHash(inputBytes);



            string passwordHash = Convert.ToHexString(hashBytes);
            query = 
                @"DECLARE @Result AS INT = -1;

                IF EXISTS (SELECT 1 FROM Users WHERE [Username] = '" + user.Username + @"' OR [Email] = '" + user.Username + @"')
                    SET @Result = 1;
                ELSE IF EXISTS (SELECT 1 FROM Users WHERE [Username] = '" + user.Email + @"' OR [Email] = '" + user.Email + @"')
                    SET @Result = 1;
                ELSE
                    SET @Result = 0;

                IF @Result = 0
                BEGIN
                    INSERT INTO Users ([FirstName], [LastName], [Username], [Email], [Password], [RoleId]) 
                    VALUES ('" + user.FirstName + @"', '" + user.LastName + @"', '" + user.Username + @"', '" + user.Email + @"', '" + passwordHash + @"', 1);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                END
                ELSE
                BEGIN
                    SELECT 0;
                END";

            int id = -1;
            SqlDataReader reader = executeQuery();
            while (reader.Read())
            {
                id = reader.GetInt32(0);
            }
            reader.Close();
            conn.Close();
            
            if (id > 0)
            {
                query = "SELECT * FROM Users WHERE [Id] = " + id;
                reader = executeQuery();

                User new_user = new User();

                while (reader.Read())
                {
                    new_user = new User() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), Username = reader.GetString(3), Email = reader.GetString(4), Password = reader.GetString(5), RoleId = reader.GetInt32(6) };
                }
                reader.Close();
                conn.Close();

                return new_user;
            }
            else
            {
                return null;
            }
        }

        public User loginUser(LoginData loginData)
        {
            MD5 hasher = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(loginData.Password);
            byte[] hashBytes = hasher.ComputeHash(inputBytes);

            string passwordHash = Convert.ToHexString(hashBytes);


            query = "SELECT * FROM Users WHERE ([Username] = '" + loginData.Login + "' AND  [Password] = '" + passwordHash + "') OR ([Email] = '" + loginData.Login + "' AND  [Password] = '" + passwordHash + "');";

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
                return null ;
            }

            return user;
        }

        public void UpdateToStudent(int userId)
        {
            query = @"UPDATE Users SET[RoleId] = 2 WHERE [Id] = " + userId;
            executeCommand();
           
        }

        public void UpdateToLecturer(int userId)
        {
            query = @"UPDATE Users SET[RoleId] = 3 WHERE [Id] = " + userId;
            executeCommand();

        }

        public void UpdateToLibrarian(int userId)
        {
            query = @"UPDATE Users SET[RoleId] = 4 WHERE [Id] = " + userId;
            executeCommand();

        }

        public List<string> GetEmailsByRoleId(int roleId)
        {
            List<string> emails = new List<string>();
            query = @"SELECT [Email] FROM Users WHERE [RoleId] = " + roleId;
            SqlDataReader reader = executeQuery();

            string email;

            while (reader.Read())
            {
               email = reader["Email"].ToString();
               emails.Add(email);
            }
            reader.Close();
            conn.Close();

            

            return emails;
        }


    }
}
