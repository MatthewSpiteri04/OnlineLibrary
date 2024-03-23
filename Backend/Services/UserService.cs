using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public class UserService : DatabaseConnection
    {
        public UserService() : base()
        {

        }

        public IEnumerable<string> getRoles()
        {
            List<string> roles = new List<string>();

            query = "SELECT [Description] FROM Roles";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                roles.Add(reader.GetString(0));
            }

            return roles;
        }

        public bool doesUserExist(string username)
        {
            query = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM Users WHERE [Username] = '" + username + "')  THEN 1 ELSE 0 END;";
            SqlDataReader reader = executeQuery();

            int flag = -1;

            while (reader.Read())
            {
               flag = reader.GetInt32(0);
            }

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
            query = @"INSERT INTO Users ([FirstName], [LastName], [Username], [Email], [Password], [RoleId]) VALUES ('" + user.FirstName + "', '" + user.LastName +"', '" + user.Username + "', '" + user.Email + "', '" + user.Password + "', " + user.RoleId + ");" +
                     "SELECT * FROM Users WHERE Id = SCOPE_IDENTITY()";
            SqlDataReader reader = executeQuery();

            User new_user = new User();

            while (reader.Read())
            {
                new_user = new User() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), Username = reader.GetString(3), Email = reader.GetString(4), Password = reader.GetString(5), RoleId = reader.GetInt32(6) } ;
            }

            return new_user;
        }

    }
}
