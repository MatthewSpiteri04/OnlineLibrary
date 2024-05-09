namespace Backend.Models
{
	public class AttributesWithTypeName
	{
		public int? Id { get; set; }
		public string Name { get; set; }
		public int? TypeId { get; set; }
		public string TypeName { get; set; }
		public bool ListView { get; set; }
	}
}
