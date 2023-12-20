using Library.DataAccess.MainModels;
using Library.Models.UserModels.Interfaces;
using System.Linq.Expressions;

namespace Library.Services.Interfaces
{
    public interface IBaseService<T> where T : IEntity
    {
        Task<int> AddAsync(T entity);
        Task<int> AddRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> IQueryableGetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<int> RemoveAsync(int id);
        Task<int> RemoveRangeAsync(IEnumerable<T> entities);
        Task<int> UpdateAsync(T entity);
        Task<int> UpdateRangeAsync(IEnumerable<T> entity);
        Task<int> GetCountOfAllItems();
        IQueryable<ApplicationUser> IQueryableGetUsersThatAreWorkers();
    }
}
