using Library.Models.BaseModels;

namespace Library.Web.Controllers.DocsControllerHelper
{
    public interface IDocsControllerHelper
    {
        IQueryable<BlogPost> GetAllDocuments();
        Task<BlogPost> EditDocHelper(Guid id);
        Task<bool> EditDocPostHelper(BlogPost doc, string fileName);
        Task<string> DeleteDocPost(Guid id);
        Task<bool> SaveDocInformation(BlogPost doc, string fileName);
    }
}
