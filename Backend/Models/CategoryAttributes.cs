namespace Backend.Models
{
	public class CategoryAttributes
	{
		public int AccessLevel { get; set; }
		public string CategoryName { get; set; }

		public List <Attributes> Attributes { get; set; }

		public int UserId { get; set; }
		
	}
}
