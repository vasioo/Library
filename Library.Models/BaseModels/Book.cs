using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class Book : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public DateTime DateOfBookCreation { get; set; }

        public string Author { get; set; } = "";

        public string Genre { get; set; } = "";

        public string Description { get; set; } = "";

        public int AvailableItems { get; set; }

        public string NeededMembership { get; set; } = "";
    }
}
