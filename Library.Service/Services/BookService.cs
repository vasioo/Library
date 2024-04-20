using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.Cloudinary;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Library.Services.Services
{
    public class BookService : BaseService<Book>, IBookService
    {
        public IConfiguration Configuration { get; }
        private readonly DataContext _dataContext;
        public BookService(IConfiguration configuration, DataContext context) : base(configuration, context)
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
                    var recommendedBooks = _dataContext.StarRatings
                        .GroupBy(r => r.Book.Id)
                        .Select(g => new
                        {
                            BookId = g.Key,
                            AverageRating = g.Average(r => r.StarCount)
                        })
                        .OrderByDescending(g => g.AverageRating)
                        .Take(6)
                        .Select(g => g.BookId);

                    return _dataContext.Books.Where(b => recommendedBooks.Contains(b.Id));

                }
            }
            var mostRated = _dataContext.StarRatings
                .GroupBy(r => r.Book.Id)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalRatings = g.Count()
                })
                .OrderByDescending(g => g.TotalRatings)
                .Take(6)
                .Select(g => g.BookId)
                .ToList();
            return _dataContext.Books.Where(b => mostRated.Contains(b.Id));
        }



        public async Task<Book> GetBookByBookName(string name)
        {
            return _dataContext.Books.Where(x => x.Title == name).FirstOrDefault();
        }

    }
}
