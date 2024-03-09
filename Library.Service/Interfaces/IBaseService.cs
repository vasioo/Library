using Library.DataAccess.MainModels;
using Library.Models.Cloudinary;
using Library.Models.UserModels.Interfaces;
using System.Linq.Expressions;

namespace Library.Services.Interfaces
{
    public interface IBaseService<T> where T : IEntity
    {
        Task<Guid> AddAsync(T entity);
        Task<int> AddRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> IQueryableGetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<int> RemoveAsync(Guid id);
        Task<int> RemoveRangeAsync(IEnumerable<T> entities);
        Task<int> UpdateAsync(T entity);
        Task<int> UpdateRangeAsync(IEnumerable<T> entity);
        Task<int> GetCountOfAllItems();
        IQueryable<ApplicationUser> IQueryableGetUsersThatAreWorkers();
        Task<bool> SaveImages(List<Photo> images);
        Task<bool> SaveImage(Photo image);
        Task<bool> DeleteImage(string imageId);
    }
}
