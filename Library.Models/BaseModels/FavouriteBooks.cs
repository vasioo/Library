namespace Library.Models.BaseModels
{
    public class FavouriteBooks
    {
        public string UserId { get; set; } = "";
        public Guid BookId { get; set; }
        public DateTime TimeOfLike { get; set; } = DateTime.Now;
    }
}
