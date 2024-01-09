using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface INotificationService : IBaseService<Notification>
    {
       Task AddDailyNotification();
        Task AddWeeklyNotification();
    }
}
