using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using System.Data.Entity;

namespace Library.Services.Services
{
    internal class UserLeasedBookService:BaseService<UserLeasedBookMappingTable>,IUserLeasedBookService
    {
        private DataContext _dataContext;

        public UserLeasedBookService(DataContext context) : base(context)
        {
            _dataContext = context;
        }
        public Task<List<string>> MostReadGenres()
        {
            var mostReadGenres = _dataContext.UserLeasedBooks
                .GroupBy(bl => bl.Book.Genre)
                .OrderByDescending(group => group.Count())
                .Take(5)
                .Select(group => group.Key);

            return Task.FromResult(mostReadGenres.ToList());
        }


        public Task<Book> MostLeasedBook()
        {
            var mostLeasedBookEntity = _dataContext.UserLeasedBooks
                .GroupBy(ulb => ulb.BookId)
                .OrderByDescending(group => group.Count())
                .FirstOrDefault();

            var mostLeasedBook = _dataContext.Books.Where(book => book.Id == mostLeasedBookEntity!.Key).FirstOrDefault() ;

            return Task.FromResult(mostLeasedBook!);
        }
    }
}
