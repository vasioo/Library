using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;

namespace Library.Models.ViewModels
{
    public class BookPageViewModel
    {
        public Book Book { get; set; } = new Book();

        public ApplicationUser User { get; set; } = new ApplicationUser();

        public IQueryable<Book> RecommendedBooks { get; set; } = Enumerable.Empty<Book>().AsQueryable();
        public IQueryable<Book> BestSellers { get; set; } = Enumerable.Empty<Book>().AsQueryable();

        public bool HasUserBorrowedIt { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsWaiting { get; set; } = false;
        public bool UserIsAuthorized { get; set; } = false;
        public bool IsBookAllowed { get; set; } = false;
        public bool IsLinkAvailable { get; set; } = false;

        public ProgressBarSettings ProgressBarSettings { get; set; } = new ProgressBarSettings();
        public int StarRatingAmount{ get; set; }
        public double AverageRate { get; set; }
    }
}
