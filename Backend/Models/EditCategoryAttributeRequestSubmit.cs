namespace Backend.Models
{
    public class EditCategoryAttributeRequestSubmit
    {
        public Categories Category { get; set; }
        public List<Attributes> Attributes { get; set; }
    }
}
