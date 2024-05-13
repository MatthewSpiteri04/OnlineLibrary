using System;
using System.Data.SqlClient;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Backend.Services;
using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections;


namespace Backend.Services
{
	public class CategoryService : DatabaseConnection
	{
		public List<AttributeTypes> getAttributeTypes()
		{
			List<AttributeTypes> attribute_types = new List<AttributeTypes>();

			query = @"SELECT * FROM AttributeTypes";
			SqlDataReader reader = executeQuery();

			while (reader.Read())
			{
				AttributeTypes attributeType = new AttributeTypes() { Id = reader.GetInt32(0), TypeName = reader.GetString(1) };
				attribute_types.Add(attributeType);
			}
			reader.Close();
            conn.Close();
            return attribute_types;
		}

		public List<AttributesWithTypeName> getAttributes()
		{
			List<AttributesWithTypeName> attributes = new List<AttributesWithTypeName>();

			query = @"SELECT A.*, [AT].TypeName FROM Attributes A
					  INNER JOIN AttributeTypes [AT] ON A.TypeId = [AT].Id";
					
			SqlDataReader reader = executeQuery();

			while (reader.Read())
			{
                AttributesWithTypeName attributeList = new AttributesWithTypeName() { Id = reader.GetInt32(0), Name = reader.GetString(1), TypeId = reader.GetInt32(2), TypeName = reader.GetString(3) };
				attributes.Add(attributeList);
			}
            reader.Close();
            conn.Close();
            return attributes;
		}

        public EditCategoryAttributeRequest getCategories(int categoryId)
        {
            Categories category = new Categories();

            query = @"SELECT * FROM Categories WHERE [Id] = " + categoryId;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                category = new Categories { Id = reader.GetInt32(0), PublicAccess = reader.GetInt32(1), Name = reader.GetString(2)};
            }

			reader.Close();
            conn.Close();

			query = @"SELECT A.*, [AT].TypeName FROM Attributes A
					  INNER JOIN CategoryAttributes CA ON A.Id = CA.AttributeId
					  INNER JOIN Categories C ON C.Id = CA.CategoryId
					  INNER JOIN AttributeTypes [AT] ON [AT].Id = A.TypeId
					  WHERE C.Id = " + categoryId;
			List<AttributesWithTypeName> attributes = new List<AttributesWithTypeName>();

			reader = executeQuery();

			while(reader.Read()) {
				attributes.Add(new AttributesWithTypeName
				{
					Id = reader.GetInt32(0),
					Name = reader.GetString(1),
					TypeId = reader.GetInt32(2),
					TypeName = reader.GetString(3)
				});
			}
            reader.Close();
            conn.Close();

			EditCategoryAttributeRequest categoryAttributeRequest = new EditCategoryAttributeRequest
			{
				Category = category,
				Attributes = attributes
			};
			
            return categoryAttributeRequest;
        }



        public int createCategory(string category, int accessLevel)
		{
			query = @"DECLARE @Result AS INT = -1;

					IF EXISTS (SELECT 1 FROM Categories WHERE [Name] = '" + category + @"')
						SET @Result = 1;
    
					ELSE
						SET @Result = 0;

					IF @Result = 0
					BEGIN
						INSERT INTO Categories ([Name], [PublicAccess]) 
						VALUES ('" + category + @"', " + accessLevel + @");

						SELECT CAST(SCOPE_IDENTITY() AS INT);

					END
					ELSE
						BEGIN
						SELECT -1;
					END";





			SqlDataReader reader = executeQuery();
			int catId = 0;
			while (reader.Read())
			{
				catId = reader.GetInt32(0);
			}
			reader.Close();
            conn.Close();
            return catId;

		}

		public int createAttributes(Attributes attribute)
		{
			query = @"
						INSERT INTO Attributes ([Name], [TypeId]) 
						VALUES ('" + attribute.Name + @"', " + attribute.TypeId + @");
						SELECT CAST(SCOPE_IDENTITY() AS INT);";


			SqlDataReader reader = executeQuery();
			int attrId = 0;
			while (reader.Read())
			{
				attrId = reader.GetInt32(0);
			}
			reader.Close();
            conn.Close();
            return attrId;

		}

		public void createCategoryAttributes(int categoryId, List<int> attributeId)
		{
			foreach (int attrId in attributeId)
			{
				query = @"INSERT INTO CategoryAttributes ([CategoryId], [AttributeId])
						  VALUES (" + categoryId + @" , " + attrId + @");";
				executeCommand();
			}
		}

		public bool checkValidAttributes(List<Attributes> attributes)
		{
			int valid = 0;
			foreach (Attributes attr in attributes)
			{
				query = @"

					IF EXISTS (SELECT 1 FROM Attributes WHERE [Name] = '" + attr.Name + @"')
						SELECT 1;
    
					ELSE
						SELECT 0;";

				SqlDataReader reader = executeQuery();
				while (reader.Read())
				{
					
					if (reader.GetInt32(0) == 1 && !attr.ListView)
					{
						valid = 1;
					}
				}
				reader.Close();
                conn.Close();

            }

			if(valid  == 1)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool checkValidCategories(string categories)
		{
			int valid = 0;
		
			
			query = @"

				IF EXISTS (SELECT 1 FROM Categories WHERE [Name] = '" + categories + @"')
					SELECT 1;
    
				ELSE
					SELECT 0;";

			SqlDataReader reader = executeQuery();
			while (reader.Read())
			{

				if (reader.GetInt32(0) == 1)
				{
					valid = 1;
				}
			}
			reader.Close();
            conn.Close();


            if (valid == 1)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool checkValidUser(int userId)
		{
			int valid = 0;


			query = @"IF EXISTS (
					  SELECT 1 FROM Users U
					  INNER JOIN Roles R ON R.Id = U.RoleId
					  INNER JOIN RolesToPrivileges RP ON R.Id = RP.RoleId
					  INNER JOIN Privileges P ON P.Id = RP.PrivilegeId
					  WHERE U.Id = " + userId + @" AND P.[Description] LIKE 'Manage Categories'
					  )
						 SELECT 1;
					  ELSE
						 SELECT 0;";


			SqlDataReader reader = executeQuery();
			while (reader.Read())
			{

				if (reader.GetInt32(0) == 1)
				{
					valid = 1;
				}
			}
			reader.Close();
            conn.Close();



            if (valid == 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public bool deleteCategory(int id)
        {
			try
			{
                query = @"	DELETE FROM CategoryAttributes WHERE CategoryId = " + id + @"; 
						DELETE FROM Categories WHERE [Id] = " + id;

                executeCommand();

                return true;
            }
			catch 
			{
				return false;
			}

			
        }

        public bool deleteAttribute(int id)
        {
            try
            {
				query = @"	DELETE FROM CategoryAttributes WHERE AttributeId = " + id; 
						

                executeCommand();

                return true;
            }
            catch
            {
                return false;
            }


        }

        public EditCategoryAttributeRequest updateCategory(EditCategoryAttributeRequestSubmit request)
        {
           
            query = @"UPDATE Categories
                      SET [Name] = '" + request.Category.Name  + @"'
                      WHERE Categories.Id =" + request.Category.Id + @";

                      SELECT * FROM Categories WHERE [Id] = " + request.Category.Id;

            Categories category = new Categories();


            SqlDataReader reader = executeQuery();
               


            while (reader.Read())
            {
                category = new Categories { Id = reader.GetInt32(0), PublicAccess = reader.GetInt32(1), Name = reader.GetString(2) };
            }

            reader.Close();
            conn.Close();

			List<AttributesWithTypeName> attribute = new List<AttributesWithTypeName>();
			query = @"	DELETE FROM CategoryAttributes WHERE CategoryId = " + request.Category.Id;



            executeCommand();

			foreach (Attributes attr in request.Attributes)
			{
				if (attr.Id == null)
				{
					query = @"INSERT INTO Attributes ([Name], [TypeId]) VALUES ('" + attr.Name + @"', " + attr.TypeId + @");
						      INSERT INTO CategoryAttributes ([CategoryId], [AttributeId])
						      VALUES (" + request.Category.Id + @", SCOPE_IDENTITY()  )";
                }
				else
				{
                    query = @"INSERT INTO CategoryAttributes ([CategoryId], [AttributeId])
						      VALUES (" + request.Category.Id + @"," + attr.Id + @")";
                }

				executeCommand();

			}
            query = @"SELECT A.*, [AT].TypeName FROM Attributes A
					  INNER JOIN CategoryAttributes CA ON A.Id = CA.AttributeId
					  INNER JOIN Categories C ON C.Id = CA.CategoryId
					  INNER JOIN AttributeTypes [AT] ON [AT].Id = A.TypeId
					  WHERE C.Id = " + request.Category.Id;

            reader = executeQuery();
			while (reader.Read())
			{
				attribute.Add(new AttributesWithTypeName
				{
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    TypeId = reader.GetInt32(2),
					TypeName = reader.GetString(3)
                });
			}
           
            reader.Close();
            conn.Close();
            EditCategoryAttributeRequest categoryAttributeUpdate = new EditCategoryAttributeRequest
            {
                Category = category,
                Attributes = attribute
            };

            return categoryAttributeUpdate;
        
        }

        public List<AccessLevels> getAccessLevels()
        {
			query = @"SELECT * FROM Roles";
			List<AccessLevels> list = new List<AccessLevels>();
			SqlDataReader reader = executeQuery();
			while(reader.Read()) 
			{
				list.Add(new AccessLevels
				{
					Id = reader.GetInt32(0),
					Description = reader.GetString(1),
				});
			}

			return list;
        }

        public bool categoryNameUnique(string name, int id)
        {
			bool bit = false;
			query = @"SELECT CAST(
					 CASE 
					   WHEN EXISTS (SELECT 1 FROM Categories WHERE Name = '" + name + @"' AND Id != " + id +  @") THEN 0 
					   ELSE 1 
					 END AS BIT
				   ) AS Result;";
			SqlDataReader reader = executeQuery();
			while (reader.Read())
			{
				bit = reader.GetBoolean(0);
			}
			reader.Close();
			conn.Close();
			return bit;
        }

        public bool attributesAreUnique(List<Attributes> attributes)
        {
            int valid = 0;
            foreach (Attributes attr in attributes)
            {
               if(attr.Id != null)
				{
                    query = @"

					IF EXISTS (SELECT 1 FROM Attributes WHERE [Name] = '" + attr.Name + @"' AND Id != " + attr.Id + @")
						SELECT 1;
    
					ELSE
						SELECT 0;";

                    SqlDataReader reader = executeQuery();
                    while (reader.Read())
                    {

                        if (reader.GetInt32(0) == 1 && !attr.ListView)
                        {
                            valid = 1;
                        }
                    }
                    reader.Close();
                    conn.Close();

                }

            }

            if (valid == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool categoryIsUsed(int id)
        {
            int valid = 0;

            query = @"

			IF EXISTS (SELECT 1 FROM Documents WHERE CategoryId = " + id + @")
				SELECT 1;
    
			ELSE
				SELECT 0;";

            SqlDataReader reader = executeQuery();
            while (reader.Read())
            {

                valid = reader.GetInt32(0);
            }
            reader.Close();
            conn.Close();


            if (valid == 1)
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
