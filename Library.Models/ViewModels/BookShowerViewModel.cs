using Library.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.ViewModels
{
    public class BookShowerViewModel
    {
        public IQueryable<Book> Books { get; set; }
        public IQueryable<Book> RecommendedBooks { get; set; }
        public IQueryable<Book> BestSellers { get; set; }

        public BookCategory CategorySortBy { get; set; } = new BookCategory();
    }
}
