using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.Cloudinary;
using Library.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using IEntity = Library.Models.UserModels.Interfaces.IEntity;

namespace Library.Services.Services
{
    public class BaseService<T> : IBaseService<T> where T : class, IEntity
    {
        public IConfiguration Configuration { get; }
        private CloudinarySettings _cloudinarySettings;
        private Cloudinary _cloudinary;
        private readonly DataContext _context;

        public BaseService(IConfiguration configuration, DataContext context)
        {
            Configuration = configuration;
            ConfigureCloudinary();
            _context = context;
        }

        public async Task ConfigureCloudinary()
        {
            _cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>() ?? new CloudinarySettings();
            Account account = new Account(
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<Guid> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            await _context.SaveChangesAsync();

            // Return the number of entities added
            return entities.Count();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public IQueryable<T> IQueryableGetAllAsync()
        {
            return _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    throw new ArgumentException("No entity was found with that id");
                }
                return entity;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> GetCountOfAllItems()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> RemoveAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return 0;

            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            if (!_context.Set<T>().Local.Any(e => e.Id == entity.Id))
            {
                _context.Set<T>().Attach(entity);
            }
            _context.Entry(entity).State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            return await _context.SaveChangesAsync();
        }

        public IQueryable<ApplicationUser> IQueryableGetUsersThatAreWorkers()
        {
            var users = (from user in _context.Users
                         join userRole in _context.UserRoles
                         on user.Id equals userRole.UserId
                         join role in _context.Roles
                         on userRole.RoleId equals role.Id
                         where role.Name == "WORKER"
                         select user);
            return users;
        }

        public async Task<bool> SaveImages(List<Photo> images)
        {
            try
            {
                foreach (var image in images)
                {
                    await _cloudinary.UploadAsync(new ImageUploadParams()
                    {
                        File = new FileDescription(image.Image),
                        DisplayName = image.ImageName,
                        PublicId = image.PublicId,
                        Overwrite = false,
                    });
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveImage(Photo image)
        {
            try
            {
                await _cloudinary.UploadAsync(new ImageUploadParams()
                {
                    File = new FileDescription(image.Image),
                    DisplayName = image.ImageName,
                    PublicId = image.PublicId,
                    Overwrite = false,
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteImage(string imageId)
        {
            try
            {
                await _cloudinary.DeleteResourcesAsync(imageId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
