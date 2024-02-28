using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class BookCategory:IEntity
    {
        public Guid Id { get; set; }

        public string CategoryName { get; set; } = "";

    }
}
