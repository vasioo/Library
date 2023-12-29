using Library.Models.BaseModels;

namespace Library.Models.ViewModels
{
    public class BookCollectionShowerViewModel
    {
        public IQueryable<Book> RecommendedBooks { get; set; }
        public IQueryable<Book> BestSellers { get; set; }
        public IQueryable<BookCategory> BookCategories { get; set; }
    }
}
