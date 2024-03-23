using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
            query = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Roles WHERE [Description] = '" + username + "')  THEN 1 ELSE 0 END;";
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

    }
}
