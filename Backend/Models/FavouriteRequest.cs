namespace Backend.Models
{
	public class FavouriteRequest
    {
		public int DocumentId { get; set; }
		public int UserId { get; set; }
		public bool IsFavourite { get; set; }
	}
}
