using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public class DocumentsService : DatabaseConnection
    {
        public DocumentsService() : base()
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

        public List<Documents> getDocuments(DocumentRequestModel request, int UserId)
        {
            List<Documents> list = new List<Documents>();

            if (UserId < 0)
            {
                query = @"SELECT 
                          D.[Id], 
                          D.[UserId],
                          C.[Name], 
                          D.[Title], 
                          L.[Language], 
                          D.[UploadDate], 
                          D.[PublicAccess], 
                          D.[DocumentLocation], 
                          D.[FileExtension], 
                          CAST(0 AS BIT) AS IsFavourite  
                      FROM 
                          Documents D
                      INNER JOIN 
                          Categories C ON D.CategoryId = C.Id
                      INNER JOIN 
                          Languages L ON D.LanguageId = L.Id
                      WHERE 
                          D.[Title] LIKE '%" + request.Search + @"%'";
            }
            else
            {
                query = @"SELECT 
                        D.[Id],
                        D.[UserId], 
                        C.[Name], 
                        D.[Title], 
                        L.[Language], 
                        D.[UploadDate], 
                        D.[PublicAccess], 
                        D.[DocumentLocation], 
                        D.[FileExtension], 
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM Favourites WHERE DocumentId = D.[Id] AND UserId = " + request.UserId + @")
                                THEN CAST(1 AS BIT)
                            ELSE CAST(0 AS BIT)
                        END AS IsFavourite  
                    FROM 
                        Documents D
                    INNER JOIN 
                        Categories C ON D.CategoryId = C.Id
                    INNER JOIN 
                        Languages L ON D.LanguageId = L.Id
                    WHERE 
                        D.[Title] LIKE '%" + request.Search + @"%'";
            }
;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                list.Add(new Documents()
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Category = reader.GetString(2),
                    Title = reader.GetString(3),
                    Language = reader.GetString(4),
                    UploadDate = reader.GetDateTime(5),
                    PublicAccess = reader.GetBoolean(6),
                    DocumentLocation = reader.GetString(7),
                    FileExtension = reader.GetString(8),
                    IsFavourite = reader.GetBoolean(9)
                });
            }
            reader.Close();
            conn.Close();

            return list;
        }

        public List<Documents> getAllDocuments(int UserId)
        {
            List<Documents> list = new List<Documents>();

            if (UserId < 0)
            { 
                query = @"SELECT D.[Id], D.[UserId], C.[Name], D.[Title], L.[Language], D.[UploadDate], D.[PublicAccess], D.[DocumentLocation], D.[FileExtension], CAST(0 AS BIT) AS IsFavourite FROM Documents D
                          INNER JOIN Categories C ON D.CategoryId = C.Id
                          INNER JOIN Languages L ON D.LanguageId = L.Id;";
            }
            else
            {
                query = @"SELECT D.[Id], D.[UserId], C.[Name], D.[Title], L.[Language], D.[UploadDate], D.[PublicAccess], D.[DocumentLocation], D.[FileExtension], CASE 
                          WHEN EXISTS (SELECT 1 FROM Favourites WHERE DocumentId = D.[Id] AND UserId = " + UserId + @")
                              THEN CAST(1 AS BIT)
                              ELSE CAST(0 AS BIT)
                              END AS IsFavourite  
                          FROM Documents D
                          INNER JOIN Categories C ON D.CategoryId = C.Id
                          INNER JOIN Languages L ON D.LanguageId = L.Id;";
            }

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                list.Add(new Documents()
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Category = reader.GetString(2),
                    Title = reader.GetString(3),
                    Language = reader.GetString(4),
                    UploadDate = reader.GetDateTime(5),
                    PublicAccess = reader.GetBoolean(6),
                    DocumentLocation = reader.GetString(7),
                    FileExtension = reader.GetString(8),
                    IsFavourite = reader.GetBoolean(9)
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
