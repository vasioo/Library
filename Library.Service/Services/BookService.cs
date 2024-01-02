using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.Cloudinary;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class BookService : BaseService<Book>, IBookService
    {
        public IConfiguration Configuration { get; }
        private CloudinarySettings _cloudinarySettings;
        private Cloudinary _cloudinary;
        private readonly DataContext _dataContext;
        public BookService(IConfiguration configuration, DataContext context) : base(context)
        {
            Configuration = configuration;
            _cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>() ?? new CloudinarySettings();
            Account account = new Account(
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _dataContext = context;
        }

        public IQueryable<Book> GetTop6BooksByCriteria(ApplicationUser user, string criteria)
        {
            if (criteria == "recommended")
            {
                if (user != null)
                {
                    var userReadBookIds = user.FavouriteBooks.Select(x => x.BookId);

                    var recommendedBooks = _dataContext.Books
                        .Where(b => !userReadBookIds.Contains(b.Id))
                        .OrderByDescending(b => b.FavouriteBooks.Count)
                        .Take(6);


                    return recommendedBooks;
                }
            }
            return _dataContext.Books.OrderBy(b => b.FavouriteBooks.Count()).Take(6);
        }

        public async Task<bool> SaveImage(int bookId, string imageUrl)
        {
            try
            {
                await _cloudinary.UploadAsync(new ImageUploadParams()
                {
                    File = new FileDescription(imageUrl),
                    DisplayName = $"image-for-book-{bookId}",
                    PublicId = $"image-for-book-{bookId}",
                    Overwrite = false,
                });
                return true;
            }
            catch (Exception)
            {
                // log error
                return false;
            }
        }

        public async Task<bool> UpdateImageData(int bookId, string imageUrl)
        {
            try
            {
                _cloudinary.DeleteResources($"image-for-book-{bookId}");

                await _cloudinary.UploadAsync(new ImageUploadParams()
                {
                    File = new FileDescription(imageUrl),
                    DisplayName = $"image-for-book-{bookId}",
                    PublicId = $"image-for-book-{bookId}",
                    Overwrite = false,
                });
                return true;
            }
            catch (Exception)
            {
                // log error
                return false;
            }
        }

        public async Task<Book> GetBookByBookName(string name)
        {
            return _dataContext.Books.Where(x => x.Name == name).FirstOrDefault();
        }
    }
}
