using Library.DataAccess.MainModels;
using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface IBookService : IBaseService<Book>
    {
        IQueryable<Book> GetTop6BooksByCriteria(ApplicationUser user, string criteria);
        Task<Book> GetBookByBookName(string name);
    }
}
