namespace Backend.Models
{
    public class DocumentWithAttribute
    {
        public Documents Document { get; set; }
        public List<DocumentAttributeValues> Attributes { get; set; }
    }
}
