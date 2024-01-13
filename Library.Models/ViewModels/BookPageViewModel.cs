using Library.DataAccess.MainModels;
using Library.Models.BaseModels;

namespace Library.Models.ViewModels
{
    public class BookPageViewModel
    {
        public Book Book { get; set; } = new Book();

        public ApplicationUser User { get; set; } = new ApplicationUser();

        public IQueryable<Book> RecommendedBooks { get; set; }
        public IQueryable<Book> BestSellers { get; set; }

        public bool HasUserBorrowedIt { get; set; }
    }
}
