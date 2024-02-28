using Library.Models.BaseModels;
using Library.Models.Cloudinary;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Library.Web.Controllers.DocsControllerHelper
{
    public class DocsControllerHelper : IDocsControllerHelper
    {
        private readonly IBlogPostService _docsService;

        public DocsControllerHelper(IBlogPostService docsService)
        {
            _docsService = docsService;
        }


        public async Task<bool> SaveDocInformation(BlogPost doc, string fileName)
        {
            try
            {
                doc.Content = HttpUtility.HtmlEncode(doc.Content);

                var id = await _docsService.AddAsync(doc);
                var photo = new Photo();
                if (fileName != null && fileName != "")
                {
                    photo.Image = fileName;
                    photo.ImageName = $"main-image-for-blog-{id}";
                    photo.PublicId = $"main-image-for-blog-{id}";
                }

                await _docsService.SaveImage(photo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IQueryable<BlogPost> GetAllDocuments()
        {
            return _docsService.IQueryableGetAllAsync();
        }

        public async Task<BlogPost> EditDocHelper(Guid id)
        {
            return await _docsService.GetByIdAsync(id);
        }

        public async Task<bool> EditDocPostHelper(BlogPost doc, string blogImage)
        {
            try
            {
                doc.Content = HttpUtility.HtmlEncode(doc.Content);
                var id = await _docsService.UpdateAsync(doc);
                //Need to add update for image
                var photo = new Photo();
                if (blogImage != null && blogImage != "")
                {
                    photo.Image = blogImage;
                    photo.ImageName = $"main-image-for-blog-{id}";
                    photo.PublicId = $"main-image-for-blog-{id}";
                }
                await _docsService.SaveImage(photo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocExists(doc.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        private bool DocExists(Guid id)
        {
            return _docsService.IQueryableGetAllAsync().Any(x => x.Id == id);
        }

        public async Task<string> DeleteDocPost(Guid id)
        {
            var doc = await _docsService.GetByIdAsync(id);

            if (doc != null)
            {
                await _docsService.RemoveAsync(doc.Id);
                return "";
            }
            return "The entity is not present in the database";
        }
    }
}
