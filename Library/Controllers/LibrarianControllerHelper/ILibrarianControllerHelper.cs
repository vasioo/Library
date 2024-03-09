using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface ILibrarianControllerHelper
    {
        Task<bool> AddABookToDatabase(BookChangersViewModel book, string image);
        Task<bool> EditABook(BookChangersViewModel book, string image);
        Task<Book> GetBook(Guid bookId);
        IQueryable<Book> GetAllBooks();
        Task<string> AddBookSubjectAndCategoriesToDb(List<BookSubjectDTO> bookSubjects, List<BookCategoryDTO> bookCategoriesDTO);
        Task<int> RemoveABook(Guid bookId);
        IQueryable<BookSubject> GetAllBookSubjects();
        Task<BookChangersViewModel> AddABookHelper();
        Task<BookChangersViewModel> EditABookHelper(Guid id);
        Task<ReportViewModel> GetReportPageModel();
        Task<IEnumerable<ReportBookDTO>> GetBookInformationByTimeAndCount(DateTime startDate, DateTime endDate, int selectedCountOfItems);
        Task<List<string>> GetGenreInformationByTimeAndCount(DateTime startDate, DateTime endDate);
        Task SaveBookHelper(BookViewModelDTO viewModelDTO); 
        Task<List<string>> GetAllGenresHelper();
        Task<LeasedTrackerViewModel> GetLeasedTrackerData(string Category);
        Task<bool> LeaseBookOrNotHelper(Guid userLeasedId, bool lease);
        Task<bool> StopLeasingHelper(Guid userLeasedId);
        Task<bool> DeleteLeasedEntityHelper(Guid id);
        Task<bool> SaveDocInformation(Document doc, string fileName);
        Task<Document> EditDocHelper(Guid id);
        Task<bool> EditDocPostHelper(Document doc, string blogImage);
        Task<string> DeleteDocPost(Guid id);
    }
}
