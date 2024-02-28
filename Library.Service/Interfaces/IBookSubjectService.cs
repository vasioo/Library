using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface IBookSubjectService:IBaseService<BookSubject>
    {
        IQueryable<BookSubject> GetBookCategoriesByBookSubject(Guid bookSubjectId); 
        int GetBookCountByBookSubject(BookSubject bookSubject);
    }
}
