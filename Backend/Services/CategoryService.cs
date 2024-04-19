using System;
using System.Data.SqlClient;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Backend.Services;


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

			return attribute_types;
		}



		public int createCategory(string category)
		{
			query = @"DECLARE @Result AS INT = -1;

					IF EXISTS (SELECT 1 FROM Categories WHERE [Name] = '" + category + @"')
						SET @Result = 1;
    
					ELSE
						SET @Result = 0;

					IF @Result = 0
					BEGIN
						INSERT INTO Categories ([Name]) 
						VALUES ('" + category + @"');

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
					
					if (reader.GetInt32(0) == 1)
					{
						valid = 1;
					}
				}
				reader.Close();
				
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
