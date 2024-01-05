using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;

namespace Library.Services.Services
{
    public class BookSubjectService : BaseService<BookSubject>, IBookSubjectService
    {
        private readonly DataContext _dataContext;

        public BookSubjectService(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public IQueryable<BookSubject> GetBookCategoriesByBookSubject(int bookSubjectId)
        {
            return _dataContext.BookSubjects.Where(x => x.Id== bookSubjectId);
        }
    }
}
