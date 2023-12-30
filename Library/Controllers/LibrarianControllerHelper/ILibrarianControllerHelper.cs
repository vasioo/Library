using Library.Models.BaseModels;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public interface ILibrarianControllerHelper
    {
        Task<bool> AddABookToDatabase(Book book);
    }
}
