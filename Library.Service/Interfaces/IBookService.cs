using Library.DataAccess.MainModels;
using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface IBookService : IBaseService<Book>
    {
        IQueryable<Book> GetTop6BooksByCriteria(ApplicationUser user, string criteria);
        Task<bool> SaveImage(int bookId, string imageUrl);
        Task<bool> UpdateImageData(int bookId, string imageUrl);
        Task<Book> GetBookByBookName(string name);
    }
}
