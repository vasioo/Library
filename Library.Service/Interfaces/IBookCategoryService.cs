using Library.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Interfaces
{
    public interface IBookCategoryService:IBaseService<BookCategory>
    {
        public BookCategory GetBookCategoryByBookCategoryName(string bookCategoryName);
    }
}
