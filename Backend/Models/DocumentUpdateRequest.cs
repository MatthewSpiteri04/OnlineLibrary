namespace Backend.Models
{
    public class DocumentUpdateRequest
    {
        public int UserId { get; set; }
        public Documents Document { get; set; }
        public List<DocumentAttributeValues> Attributes { get; set; }

    }
}
