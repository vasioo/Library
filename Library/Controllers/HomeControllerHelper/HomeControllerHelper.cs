using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Services.Interfaces;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class HomeControllerHelper : IHomeControllerHelper
    {
        private readonly INotificationService _notificationService;

        public HomeControllerHelper(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IQueryable<Notification> GetNotificationsOfTheCurrentUser(ApplicationUser receiver)
        {
            var notifications = _notificationService.IQueryableGetAllAsync();

            notifications = notifications.Where(nt => nt.Receiver == receiver).OrderByDescending(nt => nt.DateOfSending);

            return notifications;
        }
    }
}
