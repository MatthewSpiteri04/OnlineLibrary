﻿using System;
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

        public List<Documents> getMyUploads(int id)
        {
            List<Documents> list = new List<Documents>();

            query = @"SELECT D.[Id], D.[UserId], C.[Name], D.[Title], L.[Language], D.[UploadDate], D.[PublicAccess], D.[DocumentLocation], D.[FileExtension], CASE 
                          WHEN EXISTS (SELECT 1 FROM Favourites WHERE DocumentId = D.[Id] AND UserId = " + id + @")
                              THEN CAST(1 AS BIT)
                              ELSE CAST(0 AS BIT)
                              END AS IsFavourite  
                          FROM Documents D
                          INNER JOIN Categories C ON D.CategoryId = C.Id
                          INNER JOIN Languages L ON D.LanguageId = L.Id
                          WHERE D.[UserId] = " + id;

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

        public List<Documents> getMyUploadsBySearch(int id, string search)
        {
            List<Documents> list = new List<Documents>();

            query = @"SELECT D.[Id], D.[UserId], C.[Name], D.[Title], L.[Language], D.[UploadDate], D.[PublicAccess], D.[DocumentLocation], D.[FileExtension], CASE 
                          WHEN EXISTS (SELECT 1 FROM Favourites WHERE DocumentId = D.[Id] AND UserId = " + id + @")
                              THEN CAST(1 AS BIT)
                              ELSE CAST(0 AS BIT)
                              END AS IsFavourite  
                          FROM Documents D
                          INNER JOIN Categories C ON D.CategoryId = C.Id
                          INNER JOIN Languages L ON D.LanguageId = L.Id
                          WHERE D.[Title] LIKE '%" + search + @"%' AND D.[UserId] = " + id;

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

        public void deleteDocument(int id)
        {
            query = @"DELETE FROM DocumentAttributes WHERE DocumentId = " + id + @";
                      DELETE FROM Favourites WHERE DocumentId = " + id + @";
                      DELETE FROM Documents WHERE Id = " + id;

            executeCommand();
        }

        public Documents getDocumentById(int id)
        {
            Documents document = new Documents();
            query = @"SELECT D.[Id], D.[UserId], C.[Name], D.[Title], L.[Language], D.[UploadDate], D.[PublicAccess], D.[DocumentLocation], D.[FileExtension], CAST(0 AS BIT) AS IsFavourite  
                          FROM Documents D
                          INNER JOIN Categories C ON D.CategoryId = C.Id
                          INNER JOIN Languages L ON D.LanguageId = L.Id
                          WHERE D.[Id] = " + id;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                document = new Documents
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
                };
            }
            reader.Close();
            conn.Close();
            return document;
        }
    }
}
