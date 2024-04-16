using System.Data.SqlClient;

namespace Backend.Services
{
	public class CategoryService : DatabaseConnection
	{
		public List<string> getAttributeTypes()
		{
			List<string> attribute_types = new List<string>();

			query = @"SELECT [TypeName] FROM AttributeTypes";
			SqlDataReader reader = executeQuery();

			while (reader.Read())
			{
				attribute_types.Add(reader.GetString(0));
			}

			return attribute_types;
		}
	}
}
