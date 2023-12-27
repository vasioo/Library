using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Services.Interfaces;

namespace Library.Services.Services
{
    public class UserLeasedBookService : BaseService<UserLeasedBookMappingTable>, IUserLeasedBookService
    {
        private DataContext _dataContext;

        public UserLeasedBookService(DataContext context) : base(context)
        {
            _dataContext = context;
        }
        public Task<List<string>> MostReadGenres()
        {
            var totalEntityCount = _dataContext.UserLeasedBooks.Count();

            var mostReadGenres = _dataContext.UserLeasedBooks
                .GroupBy(bl => bl.Book.Genre)
                .OrderByDescending(group => group.Count())
                .Take(5)
                .AsEnumerable()
                .Select(group =>
                {
                    var genre = group.Key;
                    var count = group.Count();
                    var percentile = (double)count / totalEntityCount * 100.0;
                    return $"{genre}-{percentile:F2}";
                })
                .ToList();

            return Task.FromResult(mostReadGenres.ToList());
        }


        public Task<Book> MostLeasedBook()
        {
            var mostLeasedBookEntity = _dataContext.UserLeasedBooks
                .GroupBy(ulb => ulb.BookId)
                .OrderByDescending(group => group.Count())
                .FirstOrDefault();

            var mostLeasedBook = _dataContext.Books.Where(book => book.Id == mostLeasedBookEntity!.Key).FirstOrDefault();

            return Task.FromResult(mostLeasedBook!);
        }
    }
}
