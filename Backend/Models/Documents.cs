namespace Backend.Models
{
	public class Documents
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Author { get; set; }
		public string Category { get; set; }
		public string Title { get; set; }
		public string Language { get; set; }
		public DateTime UploadDate { get; set; }
		public int PublicAccess { get; set; }
        public string DocumentLocation { get; set; }
        public string FileExtension { get; set; }
		public bool IsFavourite { get; set; }
	}
}
