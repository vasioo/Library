using Library.DataAccess;
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

        public IQueryable<Book> GetBooks() { 
        
        }
    }
}
