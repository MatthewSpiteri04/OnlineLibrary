namespace Backend.Models
{
    public class UploadRequest
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public bool PublicAccess { get; set; }
        public IFormFile File {get; set; }
    }
}
