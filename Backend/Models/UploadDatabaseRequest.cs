namespace Backend.Models
{
    public class UploadDatabaseRequest
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public int LanguageId { get; set; }
        public DateTime UploadDate { get; set; }
        public bool PublicAccess { get; set; }
        public string DocumentLocation { get; set; }
        public string FileExtension { get; set; }
    }
}
