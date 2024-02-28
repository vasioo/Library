using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class NotificationService : BaseService<Notification>, INotificationService
    {
        private IConfiguration _configuration;
        private DataContext _dataContext;

        public NotificationService(DataContext context, IConfiguration configuration) : base(configuration, context)
        {
            _dataContext = context;
            _configuration = configuration;
        }

        public async Task AddDailyNotification()
        {
            var notification = new Notification();

            notification.DateOfSending = DateTime.Now;
            notification.Content = $"<a href=" + "Home/BookShower" + ">View today's newest collections</a>";

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
