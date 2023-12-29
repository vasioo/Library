using Library.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.ViewModels
{
    public class MainPageViewModel
    {
        public IQueryable<Book> RecommendedBooks { get; set; }
        public IQueryable<Book> BestSellers { get; set; }
    }
}
