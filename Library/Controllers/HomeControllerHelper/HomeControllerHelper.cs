using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.ViewModels;
using Library.Services.Interfaces;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class HomeControllerHelper : IHomeControllerHelper
    {
        private readonly INotificationService _notificationService;
        private readonly IBookService _bookService;

        public HomeControllerHelper(INotificationService notificationService, IBookService bookService)
        {
            _notificationService = notificationService;
            _bookService = bookService;

        }

        public BookCollectionShowerViewModel GetBookCollectionAttributes(ApplicationUser user)
        {
            var viewModel = new BookCollectionShowerViewModel();

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }

        public BookShowerViewModel GetBooksAttributes(ApplicationUser user)
        {
            var viewModel = new BookShowerViewModel();

            viewModel.Books = _bookService.IQueryableGetAllAsync();
            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }

        public MainPageViewModel GetMainPageAttributes(ApplicationUser user)
        {
            var viewModel = new MainPageViewModel();

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user,"");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }

        public IQueryable<Notification> GetNotificationsOfTheCurrentUser(ApplicationUser receiver)
        {
            var notifications = _notificationService.IQueryableGetAllAsync();

            notifications = notifications.Where(nt => nt.Receiver == receiver).OrderByDescending(nt => nt.DateOfSending);

            return notifications;
        }
    }
}
