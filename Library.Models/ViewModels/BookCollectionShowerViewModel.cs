using Library.Models.BaseModels;

namespace Library.Models.ViewModels
{
    public class BookCollectionShowerViewModel
    {
        public IQueryable<Book> RecommendedBooks { get; set; } = Enumerable.Empty<Book>().AsQueryable();
        public IQueryable<Book> BestSellers { get; set; } = Enumerable.Empty<Book>().AsQueryable();
        public IQueryable<BookSubject> BookSubjects { get; set; } = Enumerable.Empty<BookSubject>().AsQueryable();
    }
}
