using Library.Models.BaseModels;
using Library.Models.DTO;

namespace Library.Models.ViewModels
{
    public class BookShowerViewModel
    {
        public IQueryable<Book> Books { get; set; } = Enumerable.Empty<Book>().AsQueryable();
        public IQueryable<Book> RecommendedBooks { get; set; } = Enumerable.Empty<Book>().AsQueryable();
        public IQueryable<Book> BestSellers { get; set; } = Enumerable.Empty<Book>().AsQueryable();

        public BookCategory CategorySortBy { get; set; } = new BookCategory();
        public ProgressBarSettings ProgressBarSettings { get; set; } = new ProgressBarSettings();
    }
}
