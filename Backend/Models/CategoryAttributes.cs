namespace Backend.Models
{
	public class CategoryAttributes
	{
		public string CategoryName { get; set; }

		public List <Attributes> Attributes { get; set; }

		public int UserId { get; set; }
		
	}
}
