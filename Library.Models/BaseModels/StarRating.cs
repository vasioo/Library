using Library.DataAccess.MainModels;
using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class StarRating:IEntity
    {
        public Guid Id { get; set; }
        public Book Book { get; set; } = new Book();
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public int StarCount { get; set; }
    }
}
