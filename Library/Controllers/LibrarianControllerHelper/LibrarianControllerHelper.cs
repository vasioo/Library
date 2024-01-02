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

        public async Task<bool> AddABookToDatabase(Book book, string image)
        {
            try
            {
                await _bookService.AddAsync(book);
                 
                await _bookService.SaveImage(book.Id, image);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> AddABookCategoryToDatabase(string categoryName)
        {
            try
            {
                var bookCategory = new BookCategory();

                bookCategory.CategoryName = categoryName;

                await _bookCategoryService.AddAsync(bookCategory);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> EditABook(Book book)
        {
            try
            {
                if (_bookService.GetByIdAsync(book.Id) != null)
                {
                    await _bookService.UpdateAsync(book);
                    return true;
                }
                return false;
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

        public async Task<Book> GetBook(int bookId)
        {
            return await _bookService.GetByIdAsync(bookId);
        }
    }
}
