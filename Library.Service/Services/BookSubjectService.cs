using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class BookSubjectService : BaseService<BookSubject>, IBookSubjectService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public BookSubjectService(DataContext context, IConfiguration configuration) : base(configuration, context)
        {
            _dataContext = context;
            _configuration = configuration;
        }

        public IQueryable<BookSubject> GetBookCategoriesByBookSubject(Guid bookSubjectId)
        {
            return _dataContext.BookSubjects.Where(x => x.Id== bookSubjectId);
        }

        public int GetBookCountByBookSubject(BookSubject bookSubject)
        {
            var count = _dataContext.Books.Count(x => bookSubject.BookCategories.Contains(x.Genre));
            return count;

        }
    }
}
