using System.Data.SqlClient;
using Backend.Models;

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

		

	public Categories createCategory(Categories category)
		{
			query = @"DECLARE @Result AS INT = -1;

					 IF EXISTS (SELECT 1 FROM Categories WHERE [Name] = '" + category.Name + @"')
						SET @Result = 1;
    
					ELSE
						SET @Result = 0;

					IF @Result = 0
					BEGIN
						INSERT INTO Categories ([Name]) 
						VALUES ('" + category.Name + @"');
						

					END
					ELSE
					BEGIN
						SELECT 0;
					END";





			SqlDataReader reader = executeQuery();
			return category;
			
		}

		public Attributes createAttributes(Attributes attribute)
		{
			query = @"
						INSERT INTO Attributes ([Name], [TypeId]) 
						VALUES ('" + attribute.Name + @"', '" + attribute.TypeId + @"');";	





			SqlDataReader reader = executeQuery();
			return attribute;

		}


	}
}
