using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;

namespace Library.Services.Services
{
    public class NotificationService : BaseService<Notification>, INotificationService
    {
        private DataContext _dataContext;

        public NotificationService(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public async Task AddDailyNotification()
        {
            var notification = new Notification();

            notification.DateOfSending = DateTime.Now;
            notification.Content = $"<a href="+ "Home/BookShower" + ">View today's newest collections</a>";

            await _dataContext.Notifications.AddAsync(notification);
        }

        public async Task AddWeeklyNotification()
        {
            var notification = new Notification();

            notification.DateOfSending = DateTime.Now;
            notification.Content = $"<a href=" + "Home/BookShower" + ">View this week's newest collections</a>";

            await _dataContext.Notifications.AddAsync(notification);
        }
    }
}
