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

        public Permissions getRoles(int id)
        {
            Permissions user_permissions = new Permissions();

            query = @"SELECT R.* FROM Roles R
                      INNER JOIN Users U
                      ON R.Id = U.RoleId
                      WHERE U.Id = " + id;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                user_permissions = new Permissions() { Id = reader.GetInt32(0), Description = reader.GetString(1), AcademicUser = reader.GetBoolean(2), ManageCategories = reader.GetBoolean(3)};
            }

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
                    VALUES ('" + user.FirstName + @"', '" + user.LastName + @"', '" + user.Username + @"', '" + user.Email + @"', '" + user.Password + @"', 1);
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
            
            if (id > 0)
            {
                query = "SELECT * FROM Users WHERE [Id] = " + id;
                reader = executeQuery();

                User new_user = new User();

                while (reader.Read())
                {
                    new_user = new User() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), Username = reader.GetString(3), Email = reader.GetString(4), Password = reader.GetString(5), RoleId = reader.GetInt32(6) };
                }

                return new_user;
            }
            else
            {
                return null;
            }
        }

        public User loginUser(LoginData loginData)
        { 
            query = "SELECT * FROM Users WHERE ([Username] = '" + loginData.Login + "' AND  [Password] = '" + loginData.Password + "') OR ([Email] = '" + loginData.Login + "' AND  [Password] = '" + loginData.Password + "');";

            SqlDataReader reader = executeQuery();

            User user = new User();

            while (reader.Read())
            {
                user = new User() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), Username = reader.GetString(3), Email = reader.GetString(4), Password = reader.GetString(5), RoleId = reader.GetInt32(6) };
            }

            if (user.Id <= 0)
            {
                return null ;
            }

            return user;
        }

    }
}
