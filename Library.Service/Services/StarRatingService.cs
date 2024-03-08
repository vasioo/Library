using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class StarRatingService:BaseService<StarRating>,IStarRatingService
    {
        private DataContext _dataContext;
        private readonly IConfiguration _configuration;
        public StarRatingService(IConfiguration configuration, DataContext context) : base(configuration, context)
        {
            _configuration = configuration;
            _dataContext = context;
        }
    }
}
