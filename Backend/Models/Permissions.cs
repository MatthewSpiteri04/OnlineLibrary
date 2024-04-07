namespace Backend.Models
{
    public class Permissions
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool? AcademicUser { get; set; }
        public bool? ManageCategories { get; set; }
    }
}
