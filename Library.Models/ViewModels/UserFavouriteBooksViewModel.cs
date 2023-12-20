using Library.DataAccess.MainModels;
using Library.Models.BaseModels;

namespace Library.Models.ViewModels
{
    public class UserFavouriteBooksViewModel
    {
        public ApplicationUser User { get; set; } = new ApplicationUser();

        public IEnumerable<Book> Books { get; set; } = Enumerable.Empty<Book>();    
    }
}
