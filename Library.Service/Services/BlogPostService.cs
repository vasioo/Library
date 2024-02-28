using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class BlogPostService : BaseService<BlogPost>, IBlogPostService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public BlogPostService(DataContext context, IConfiguration configuration) : base(configuration, context)
        {
            _dataContext = context;
            _configuration = configuration;

        }
    }
}
