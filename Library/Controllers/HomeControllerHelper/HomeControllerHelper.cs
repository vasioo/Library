using Hangfire;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.ViewModels;
using Library.Services.Interfaces;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class HomeControllerHelper : IHomeControllerHelper
    {
        #region Fields&Constructor
        private readonly INotificationService _notificationService;
        private readonly IBookService _bookService;
        private readonly IBookCategoryService _bookCategoryService;
        private readonly IUserLeasedBookService _userLeasedBookService;
        private readonly IBookSubjectService _bookSubjectService;

        public HomeControllerHelper(INotificationService notificationService, IBookSubjectService  bookSubjectService,
            IBookService bookService, IBookCategoryService bookCategoryService, IUserLeasedBookService userLeasedBookService)
        {
            _notificationService = notificationService;
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
            _userLeasedBookService = userLeasedBookService;
            _bookSubjectService = bookSubjectService;
        }
        #endregion

        #region BookCollectionShowerHelper
        public BookCollectionShowerViewModel GetBookCollectionAttributes(ApplicationUser user)
        {
            var viewModel = new BookCollectionShowerViewModel();

            viewModel.BookSubjects = _bookSubjectService.IQueryableGetAllAsync().OrderBy(x=>x.SubjectName);
            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }
        #endregion

        #region BookShowerHelper
        public BookShowerViewModel GetBooksAttributes(ApplicationUser user, string category)
        {
            var viewModel = new BookShowerViewModel();

            var cat = _bookCategoryService.IQueryableGetAllAsync().Where(x => x.CategoryName == category).FirstOrDefault();

            viewModel.CategorySortBy = cat == null ? new BookCategory() : cat;
            viewModel.Books = _bookService.IQueryableGetAllAsync().Where(x => x.Genre == viewModel.CategorySortBy);

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");


            return viewModel;
        }
        #endregion

        #region MainPageHelper
        public MainPageViewModel GetMainPageAttributes(ApplicationUser user)
        {
            RecurringJob.AddOrUpdate(() => _notificationService.AddDailyNotification(), "0 14 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => _notificationService.AddWeeklyNotification(), "0 17 * * 0", TimeZoneInfo.Local);
            var viewModel = new MainPageViewModel();

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }
        #endregion

        #region NotificationsHelper
        public IQueryable<Notification> GetNotifications()
        {
            var notifications = _notificationService.IQueryableGetAllAsync();
            return notifications;
        }
        #endregion

        #region BookPageHelper
        public async Task<BookPageViewModel> GetBookPageAttributes(ApplicationUser user, int bookId)
        {
            var viewModel = new BookPageViewModel();

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            viewModel.User = user;
            viewModel.Book = await _bookService.GetByIdAsync(bookId);

            return viewModel;
        }
        #endregion

        #region BorrowedHelper
        public BorrowedViewModel GetBorrowedPageAttributes(ApplicationUser user)
        {
            var viewModel = new BorrowedViewModel();

            var allLeasedBooks = _userLeasedBookService.IQueryableGetAllAsync();

            viewModel.BorrowedBooks = allLeasedBooks.Where(us => us.UserId == user.Id).Select(x => x.Book);

            return viewModel;
        }
        #endregion
    }
}
