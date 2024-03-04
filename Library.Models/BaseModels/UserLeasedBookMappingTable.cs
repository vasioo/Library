using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class UserLeasedBookMappingTable:IEntity
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = "";

        public Book Book { get; set; } = new Book();

        public DateTime DateOfBorrowing { get; set; } = DateTime.Now;
    }
}
