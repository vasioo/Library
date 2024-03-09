using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface IHomeControllerHelper
    {
        MainPageViewModel GetMainPageAttributes(ApplicationUser user);
        BookCollectionShowerViewModel GetBookCollectionAttributes(ApplicationUser user);
        BookShowerViewModel GetBooksAttributes(ApplicationUser user, string category);
        Task<BookPageViewModel> GetBookPageAttributes(ApplicationUser user, Guid bookId);
        BorrowedViewModel GetBorrowedPageAttributes(ApplicationUser user);
        Task<bool> BorrowBookPostHelper(Guid bookId, ApplicationUser userId);
        Task<bool> UnborrowBookPostHelper(Guid bookId, string userId);
        Task<SearchViewModel> SearchViewModelHelper(string searchCategory, string inputValue,int page);
        Task<bool> RateBookHelper(int stars, Guid bookId, ApplicationUser user);
        Task<bool> SubmitUserFeedbackHelper(UserFeedbackDTO userFeedback, ApplicationUser user);
        ProgressBarSettings ProgressBarInformationFiller(ApplicationUser user);
        Task<Document> GetDocumentPageEntity(Guid id);
    }
}
