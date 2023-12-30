using Library.Models.BaseModels;
using Library.Services.Interfaces;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class LibrarianControllerHelper : ILibrarianControllerHelper
    {
        private readonly IBookService _bookService;

        public LibrarianControllerHelper(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<bool> AddABookToDatabase(Book book)
        {
            try
            {
                await _bookService.UpdateAsync(book);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
