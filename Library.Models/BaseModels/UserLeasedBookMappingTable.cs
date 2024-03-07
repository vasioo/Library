using Library.DataAccess.MainModels;
using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class UserLeasedBookMappingTable:IEntity
    {
        public Guid Id { get; set; }

        public ApplicationUser User { get; set; } = new ApplicationUser();

        public Book Book { get; set; } = new Book();

        public DateTime DateOfBorrowing { get; set; } = DateTime.Now;

        public bool Approved { get; set; } = false;

        public bool IsRead { get; set; } = false;
    }
}
