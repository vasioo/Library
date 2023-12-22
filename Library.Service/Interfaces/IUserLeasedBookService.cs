using Library.DataAccess.MainModels;
using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface IUserLeasedBookService:IBaseService<UserLeasedBookMappingTable>
    {
        Task<Book> MostLeasedBook();
        Task<List<string>> MostReadGenres();
    }
}
