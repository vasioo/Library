using System.ComponentModel.DataAnnotations;

namespace Library.Models.ViewModels
{
    public class BookChangersViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        [DataType(DataType.Date)]
        public DateTime DateOfBookCreation { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBookPublishment { get; set; } = DateTime.Now;
        public string Genre { get; set; } = "";
        public string Description { get; set; } = "";
        public string NeededMembership { get; set; } = "";
        public int AmountOfBooks { get; set; }
        public string ISBN { get; set; } = "";
        public string Language { get; set; } = "";
        public string PreviewLink { get; set; } = "";
        public IQueryable<string> AllGenres { get; set; } = Enumerable.Empty<string>().AsQueryable();
        public IQueryable<string> AllMemberships { get; set; } = Enumerable.Empty<string>().AsQueryable();
    }
}
