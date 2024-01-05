using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface IBookSubjectService:IBaseService<BookSubject>
    {
        IQueryable<BookSubject> GetBookCategoriesByBookSubject(int bookSubjectId);
    }
}
