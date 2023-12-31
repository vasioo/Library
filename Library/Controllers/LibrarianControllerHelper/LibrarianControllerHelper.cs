using Library.Models.BaseModels;
using Library.Services.Interfaces;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class LibrarianControllerHelper : ILibrarianControllerHelper
    {
        private readonly IBookService _bookService;
        private readonly IBookCategoryService _bookCategoryService;

        public LibrarianControllerHelper(IBookService bookService, IBookCategoryService bookCategoryService)
        {
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
        }

        public async Task<bool> AddABookToDatabase(Book book)
        {
            try
            {
                await _bookService.AddAsync(book);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public IQueryable<string> GetAllBookCategories()
        {
            return _bookCategoryService.IQueryableGetAllAsync().Select(x=>x.CategoryName);
        }
    }
}
