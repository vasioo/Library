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

        public Task<MainPageViewModel> GetMainPageAttributes(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Notification> GetNotificationsOfTheCurrentUser(ApplicationUser receiver)
        {
            var notifications = _notificationService.IQueryableGetAllAsync();

            notifications = notifications.Where(nt => nt.Receiver == receiver).OrderByDescending(nt => nt.DateOfSending);

            return notifications;
        }
    }
}
