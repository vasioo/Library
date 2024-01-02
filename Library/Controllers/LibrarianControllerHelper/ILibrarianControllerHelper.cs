using Library.Models.BaseModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface ILibrarianControllerHelper
    {
        Task<bool> AddABookToDatabase(Book book,string image);
        Task<bool> EditABook(Book book);
        IQueryable<string> GetAllBookCategories();
        Task<Book> GetBook(int bookId);
        Task<bool> AddABookCategoryToDatabase(string categoryName);
    }
}
