namespace Backend.Models
{
    public class EditCategoryAttributeRequest
    {
        public Categories Category { get; set; }
       
        public List<Attributes> Attributes { get; set; }
    }
}
