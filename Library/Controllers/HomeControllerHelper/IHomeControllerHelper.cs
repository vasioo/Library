using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.ViewModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface IHomeControllerHelper
    {
        IQueryable<Notification> GetNotificationsOfTheCurrentUser(ApplicationUser receiver);
        Task<MainPageViewModel> GetMainPageAttributes(ApplicationUser user);
    }
}
