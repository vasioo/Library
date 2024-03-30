using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class UserLeasedBookService : BaseService<UserLeasedBookMappingTable>, IUserLeasedBookService
    {
        private DataContext _dataContext;
        private IConfiguration _configuration;

        public UserLeasedBookService(DataContext context, IConfiguration configuration) : base(configuration, context)
        {
            _dataContext = context;
            _configuration = configuration;
        }
        public async Task<List<string>> MostReadGenres(DateTime startDate, DateTime endDate)
        {
            var leasedBooks = await _dataContext.UserLeasedBooks
                .Where(x => x.DateOfBorrowing >= startDate && x.DateOfBorrowing <= endDate && x.Book.Genre != null)
                .ToListAsync();
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
                .GroupBy(ulb => ulb.Book.Id)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefaultAsync();

            if (mostLeasedBookId != Guid.Empty)
            {
                var mostLeasedBook = await _dataContext.Books
                    .Where(book => book.Id == mostLeasedBookId)
                    .FirstOrDefaultAsync();

                return mostLeasedBook;
            }
            return new Book();
        }

        public async Task<UserLeasedBookMappingTable?> GetBorrowedBookByUserIdAndBookId(Guid bookId, string userId)
        {
            var model = await _dataContext.UserLeasedBooks.Where(x => x.User.Id == userId && x.Book.Id == bookId).FirstOrDefaultAsync();
            if (model != null)
            {
                return model;
            }
            return new UserLeasedBookMappingTable();
        }

        public async Task<IEnumerable<ReportBookDTO>> GetBooksInformationByTimeAndCountOfItems(DateTime startTimeSpan, DateTime endTimeSpan, int selectedCountOfItems)
        {
            var mostLeasedBookIds = _dataContext.UserLeasedBooks
                .Where(x => x.DateOfBorrowing >= startTimeSpan && x.DateOfBorrowing <= endTimeSpan)
                .GroupBy(ulb => ulb.Book.Id)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .Take(selectedCountOfItems);

            var items = new List<ReportBookDTO>();

            foreach (var item in mostLeasedBookIds)
            {
                var bookEntity = _dataContext.Books.Where(x => x.Id == item).FirstOrDefault();
                var rbEntity = new ReportBookDTO();
                rbEntity.Id = bookEntity.Id;
                rbEntity.Name = bookEntity.Title;
                items.Add(rbEntity);
            }
            return items;
        }

        public IQueryable<UserLeasedBookMappingTable> GetActiveLeasedBooks()
        {
            return _dataContext.UserLeasedBooks
                .Where(x => x.DateOfBorrowing
                    .AddHours(_dataContext.Memberships.Where(z => z.StartingNeededAmountOfPoints <= x.User.Points).Count())
                                >= DateTime.Now && x.Approved && x.IsRead);
        }

        public IQueryable<UserLeasedBookMappingTable> GetExpiredLeasedBooks()
        {
            return _dataContext.UserLeasedBooks
                .Where(x => x.DateOfBorrowing
                    .AddHours(_dataContext.Memberships.Where(z => z.StartingNeededAmountOfPoints <= x.User.Points).Count())
                                < DateTime.Now && x.IsRead);
        }

        public async Task RemoveAnHourToExistingEntity(Guid id)
        {
            var entity = await _dataContext.UserLeasedBooks.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.DateOfBorrowing = DateTime.Now.AddHours(-1);
                entity.IsRead = true;
                entity.Approved = false;
                _dataContext.Update(entity);
            }

            await _dataContext.SaveChangesAsync();
        }

        public async Task<string> GetLeasedBookStatus(Guid bookId, ApplicationUser user)
        {
            var leasedItem = await _dataContext.UserLeasedBooks.Where(x => x.Book.Id == bookId && x.User.Id == user.Id).FirstOrDefaultAsync();
            if (leasedItem != null)
            {
                if (leasedItem.Approved && leasedItem.DateOfBorrowing
                                .AddHours(_dataContext.Memberships.Where(z => z.StartingNeededAmountOfPoints <= user.Points).Count()) >= DateTime.Now)
                {
                    return "Active";
                }
                else if (leasedItem.Approved && leasedItem.DateOfBorrowing
                                .AddHours(_dataContext.Memberships.Where(z => z.StartingNeededAmountOfPoints <= user.Points).Count()) < DateTime.Now)
                {
                    return "Expired";
                }
            }
            return "";
        }

    }
}
