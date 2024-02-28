using Library.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.DTO
{
    public class BookDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        [DataType(DataType.Date)]
        public DateTime DateOfBookCreation { get; set; }
        public BookCategory? Genre { get; set; }
        public string Description { get; set; } = "";
        public int AvailableItems { get; set; }
        public string NeededMembership { get; set; } = "";
        public ICollection<FavouriteBooks> FavouriteBooks { get; set; }
    }
}
