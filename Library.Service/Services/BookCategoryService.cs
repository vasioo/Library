using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class BookCategoryService : BaseService<BookCategory>, IBookCategoryService
    {
        private DataContext _dataContext;
        private readonly IConfiguration _configuration;
        public BookCategoryService(IConfiguration configuration, DataContext context) : base(configuration, context)
        {
            _configuration = configuration;
            _dataContext = context;
        }

        public BookCategory GetBookCategoryByBookCategoryName(string bookCategoryName)
        {
            return _dataContext.Categories.Where(x => x.CategoryName! == bookCategoryName!).FirstOrDefault();
        }
    }
}
