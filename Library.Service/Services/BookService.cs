using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.Cloudinary;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class BookService : BaseService<Book>, IBookService
    {
        public IConfiguration Configuration { get; }
        private readonly DataContext _dataContext;
        public BookService(IConfiguration configuration, DataContext context) : base(configuration,context)
        {
            Configuration = configuration;
            _dataContext = context;
        }

        public IQueryable<Book> GetTop6BooksByCriteria(ApplicationUser user, string criteria)
        {
            if (criteria == "recommended")
            {
                if (user != null)
                {
                    if (user.FavouriteBooks != null)
                    {
                        var userReadBookIds = user.FavouriteBooks.Select(x => x.BookId);

                        return _dataContext.Books
                            .Where(b => !userReadBookIds.Contains(b.Id))
                            .OrderByDescending(b => b.FavouriteBooks.Count)
                            .Take(6);
                    }

                    return _dataContext.Books
                           .OrderByDescending(b => b.FavouriteBooks.Count)
                           .Take(6);
                }
            }
            return _dataContext.Books.OrderBy(b => b.FavouriteBooks.Count()).Take(6);
        }

       
        public async Task<Book> GetBookByBookName(string name)
        {
            return _dataContext.Books.Where(x => x.Title == name).FirstOrDefault();
        }

    }
}
