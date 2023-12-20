using Library.Models.BaseModels;

namespace Library.Models.ViewModels
{
    public class BookVisualiserViewModel
    {
        public IEnumerable<Book> Books{ get; set; } = Enumerable.Empty<Book>();
    }
}
