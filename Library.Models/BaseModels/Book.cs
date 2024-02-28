using Library.Models.UserModels.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.BaseModels
{
    public class Book : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        [DataType(DataType.Date)]
        public DateTime DateOfBookCreation { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBookPublishment { get; set; } = DateTime.Now;
        public BookCategory? Genre { get; set; }
        public string Description { get; set; } = "";
        public int AvailableItems { get; set; }
        public string NeededMembership { get; set; } = "";
        public ICollection<FavouriteBooks> FavouriteBooks { get; set; }
    }
}
