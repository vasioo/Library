using Library.DataAccess.MainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.BaseModels
{
    public class FavouriteBooks
    {
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public Book Book { get; set; } = new Book();
        public DateTime TimeOfLike { get; set; } = DateTime.Now;
    }
}
