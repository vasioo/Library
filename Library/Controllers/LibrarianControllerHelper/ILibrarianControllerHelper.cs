using Library.Models.BaseModels;
using Library.Models.DTO;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface ILibrarianControllerHelper
    {
        Task<bool> AddABookToDatabase(BookDTO book,string image);
        Task<bool> EditABook(BookDTO book,string image);
        IQueryable<string> GetAllBookCategories();
        Task<Book> GetBook(int bookId);
        IQueryable<Book> GetAllBooks();
        Task<string> AddBookSubjectAndCategoriesToDb(List<BookSubjectDTO> bookSubjects, List<BookCategoryDTO> bookCategoriesDTO);
        Task<int> RemoveABook(int bookId);
        IQueryable<BookSubject> GetAllBookSubjects();
    }
}
