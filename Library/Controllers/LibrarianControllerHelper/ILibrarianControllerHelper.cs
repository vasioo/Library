using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface ILibrarianControllerHelper
    {
        Task<bool> AddABookToDatabase(BookChangersViewModel book,string image);
        Task<bool> EditABook(BookChangersViewModel book,string image);
        Task<Book> GetBook(Guid bookId);
        IQueryable<Book> GetAllBooks();
        Task<string> AddBookSubjectAndCategoriesToDb(List<BookSubjectDTO> bookSubjects, List<BookCategoryDTO> bookCategoriesDTO);
        Task<int> RemoveABook(Guid bookId);
        IQueryable<BookSubject> GetAllBookSubjects();
        Task<BookChangersViewModel> AddABookHelper();
        Task<BookChangersViewModel> EditABookHelper(Guid id);
    }
}
