using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Services
{
    public class BookCategoryService : BaseService<BookCategory>, IBookCategoryService
    {
        private DataContext _context;
        public BookCategoryService(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
