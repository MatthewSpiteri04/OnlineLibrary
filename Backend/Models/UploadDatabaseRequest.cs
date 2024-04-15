namespace Backend.Models
{
    public class UploadDatabaseRequest
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public DateTime UploadDate { get; set; }
        public bool PublicAccess { get; set; }
        public string DocumentLocation { get; set; }
    }
}
