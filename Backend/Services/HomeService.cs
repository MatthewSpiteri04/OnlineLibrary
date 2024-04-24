using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Services
{
    public class HomeService : DatabaseConnection
    {
        public HomeService() : base()
        {

        }

        public List<CategoryRequest> getCategories()
        {
            List<CategoryRequest> list = new List<CategoryRequest>();

            query = @"SELECT * FROM Categories";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                list.Add(new CategoryRequest()
                {
                    Id = reader.GetInt32(0),
                    Type = reader.GetString(1)
                });
            }
            reader.Close();
            conn.Close();

            return list;
        }

        public List<LanguageRequest> getLanguages()
        {
            List<LanguageRequest> list = new List<LanguageRequest>();

            query = @"SELECT * FROM Languages";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                list.Add(new LanguageRequest()
                {
                    Id = reader.GetInt32(0),
                    Language = reader.GetString(1)
                });
            }
            reader.Close();
            conn.Close();

            return list;
        }

        public void toggleFavourite(FavouriteRequest request)
        {
            if (!request.IsFavourite)
            {
                query = @"INSERT INTO Favourites([DocumentId], [UserId]) VALUES (" + request.DocumentId + @", " + request.UserId + @");";
            }
            else
            {
                query = @"DELETE FROM Favourites WHERE [DocumentId] = " + request.DocumentId + @" AND  [UserId] = " + request.UserId;
            }

            executeCommand();
        }
    }
}
