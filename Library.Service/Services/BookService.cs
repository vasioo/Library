using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Services.Interfaces;

namespace Library.Services.Services
{
    public class BookService : BaseService<Book>, IBookService
    {
        private DataContext _dataContext;
        public BookService(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public IQueryable<Book> GetTop6BooksByCriteria(ApplicationUser user, string criteria)
        {
            if (criteria == "recommended")
            {
                if (user!=null)
                {
                    var userReadBookIds = user.FavouriteBooks.Select(x => x.BookId);

                    var recommendedBooks = _dataContext.Books
                        .Where(b => !userReadBookIds.Contains(b.Id))
                        .OrderByDescending(b => b.FavouriteBooks.Count)
                        .Take(6);


                    return recommendedBooks;
                }
            }
            return _dataContext.Books.OrderBy(b => b.FavouriteBooks.Count()).Take(6);
        }
    }
}
