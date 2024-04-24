using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public class FavouritesService : DatabaseConnection
    {
        public FavouritesService() : base()
        {

        }

        public List<Documents> getFavourites(int id)
        {
            List<Documents> list = new List<Documents>();

            query = @"SELECT D.[Id], C.[Name], D.[Title], L.[Language], D.[UploadDate], D.[PublicAccess], D.[DocumentLocation], CASE 
                      WHEN EXISTS (SELECT 1 FROM Favourites WHERE DocumentId = D.[Id] AND UserId = " + id + @")
                          THEN CAST(1 AS BIT)
                          ELSE CAST(0 AS BIT)
                          END AS IsFavourite 
                    FROM Favourites F
                    INNER JOIN Documents D ON F.DocumentId = D.Id
                    INNER JOIN Categories C ON D.CategoryId = C.Id
                    INNER JOIN Languages L ON D.LanguageId = L.Id
                    WHERE F.UserId = " + id;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                list.Add(new Documents()
                {
                    Id = reader.GetInt32(0),
                    Category = reader.GetString(1),
                    Title = reader.GetString(2),
                    Language = reader.GetString(3),
                    UploadDate = reader.GetDateTime(4),
                    PublicAccess = reader.GetBoolean(5),
                    DocumentLocation = reader.GetString(6),
                    IsFavourite = reader.GetBoolean(7)
                });
            }
            reader.Close();
            conn.Close();

            return list;
        }
    }
}
