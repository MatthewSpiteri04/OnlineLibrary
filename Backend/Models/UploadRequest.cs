namespace Backend.Models
{
    public class UploadRequest
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public int LanguageId { get; set; }
        public int PublicAccess { get; set; }
        public IFormFile File {get; set; }
        public string AttributesListJSON { get; set; }
    }
}
