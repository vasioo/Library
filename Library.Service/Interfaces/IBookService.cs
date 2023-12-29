using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Interfaces
{
    public interface IBookService : IBaseService<Book>
    {
        IQueryable<Book> GetTop6BooksByCriteria(ApplicationUser user, string criteria);
    }
}
