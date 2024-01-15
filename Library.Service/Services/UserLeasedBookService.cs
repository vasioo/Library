using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Services
{
    public class UserLeasedBookService : BaseService<UserLeasedBookMappingTable>, IUserLeasedBookService
    {
        private DataContext _dataContext;

        public UserLeasedBookService(DataContext context) : base(context)
        {
            _dataContext = context;
        }
        public async Task<List<string>> MostReadGenres()
        {
            var leasedBooks = await _dataContext.UserLeasedBooks.ToListAsync();
            var totalEntityCount = leasedBooks.Count;

            var mostReadGenres = leasedBooks
                .GroupBy(bl => bl.Book.Genre)
                .OrderByDescending(group => group.Count())
                .Take(5)
                .Select(group =>
                {
                    var genre = group.Key;
                    var count = group.Count();
                    var percentile = (double)count / totalEntityCount * 100.0;
                    return $"{genre.CategoryName}-{percentile:F2}";
                })
                .ToList();

            return mostReadGenres;
        }


        public async Task<Book> MostLeasedBook()
        {
            var mostLeasedBookId = await _dataContext.UserLeasedBooks
                .GroupBy(ulb => ulb.BookId)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefaultAsync();

            if (mostLeasedBookId != null)
            {
                var mostLeasedBook = await _dataContext.Books
                    .Where(book => book.Id == mostLeasedBookId)
                    .FirstOrDefaultAsync();

                return mostLeasedBook;
            }

            // Handle the case when there are no leased books
            return null;
        }



        public async Task<UserLeasedBookMappingTable?> GetBorrowedBookByUserIdAndBookId(int bookId, string userId)
        {
            var model = await _dataContext.UserLeasedBooks.Where(x => x.UserId == userId && bookId == bookId).FirstOrDefaultAsync();
            if (model!=null)
            {
                return model;
            }
            return new UserLeasedBookMappingTable();
        }
    }
}
