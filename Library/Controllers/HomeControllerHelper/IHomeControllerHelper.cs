using Library.DataAccess.MainModels;
using Library.Models.BaseModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface IHomeControllerHelper
    {
        IQueryable<Notification> GetNotificationsOfTheCurrentUser(ApplicationUser receiver)
    }
}
