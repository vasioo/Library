using Library.Models.BaseModels;
using Library.Models.DTO;
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

        public async Task<bool> AddABookToDatabase(BookDTO book, string image)
        {
            try
            {
                var bookCat = _bookCategoryService.GetBookCategoryByBookCategoryName(book.Genre!.CategoryName);

                var bookNew = new Book();

                bookNew.Id = book.Id;
                bookNew.Name = book.Name;
                bookNew.Author = book.Author;
                bookNew.DateOfBookCreation = book.DateOfBookCreation;
                bookNew.Genre = bookCat;
                bookNew.Description = book.Description;
                bookNew.AvailableItems = book.AvailableItems;
                bookNew.NeededMembership = book.NeededMembership;
                bookNew.FavouriteBooks = book.FavouriteBooks;

                await _bookService.AddAsync(bookNew);

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

        public async Task<bool> EditABook(BookDTO book, string imageObj)
        {
            try
            {
                var bookCat = _bookCategoryService.GetBookCategoryByBookCategoryName(book.Genre!.CategoryName);

                var bookNew = new Book();

                bookNew.Id = book.Id;
                bookNew.Name = book.Name;
                bookNew.Author = book.Author;
                bookNew.DateOfBookCreation = book.DateOfBookCreation;
                bookNew.Genre = bookCat;
                bookNew.Description = book.Description;
                bookNew.AvailableItems = book.AvailableItems;
                bookNew.NeededMembership = book.NeededMembership;
                bookNew.FavouriteBooks = book.FavouriteBooks;


                await _bookService.UpdateAsync(bookNew);

                await _bookService.UpdateImageData(book.Id, imageObj);

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
            return _bookCategoryService.IQueryableGetAllAsync().Select(x => x.CategoryName);
        }

        public async Task<Book> GetBook(int bookId)
        {
            return await _bookService.GetByIdAsync(bookId);
        }

        public IQueryable<Book> GetAllBooks()
        {
            return _bookService.IQueryableGetAllAsync();
        }

        public async Task<int> RemoveABook(int bookId)
        {
            return await _bookService.RemoveAsync(bookId);
        }
    }
}
