using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.ViewModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface IHomeControllerHelper
    {
        IQueryable<Notification> GetNotificationsOfTheCurrentUser(ApplicationUser receiver);
        MainPageViewModel GetMainPageAttributes(ApplicationUser user);
        BookCollectionShowerViewModel GetBookCollectionAttributes(ApplicationUser user);
        BookShowerViewModel GetBooksAttributes(ApplicationUser user, string category);
        Task<BookPageViewModel> GetBookPageAttributes(ApplicationUser user, int bookId);
        BorrowedViewModel GetBorrowedPageAttributes(ApplicationUser user)
    }
}
