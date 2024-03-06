using Library.Models.BaseModels;
using Library.Models.DTO;

namespace Library.Services.Interfaces
{
    public interface IUserLeasedBookService:IBaseService<UserLeasedBookMappingTable>
    {
        Task<Book> MostLeasedBook();
        Task<List<string>> MostReadGenres(DateTime startDate, DateTime endDate);
        Task<UserLeasedBookMappingTable?> GetBorrowedBookByUserIdAndBookId(Guid bookId, string userId);
        Task<IEnumerable<ReportBookDTO>> GetBooksInformationByTimeAndCountOfItems(DateTime startTimeSpan, DateTime endTimeSpan, int selectedCountOfItems);
        IQueryable<UserLeasedBookMappingTable> GetActiveLeasedBooks();
        IQueryable<UserLeasedBookMappingTable> GetExpiredLeasedBooks();
        Task RemoveAnHourToExistingEntity(Guid id);
    }
}
