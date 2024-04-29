using System;
using System.Data.SqlClient;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Backend.Services;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


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
                      SET [FirstName] = '" + request.FirstName + @"', [LastName] = '" + request.LastName + @"', [Username] = '" + request.Username + @"', [Email] = '" + request.Email + @"', [Password] = '" + request.Password + @"'
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
    }
}
