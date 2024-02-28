using Library.Models.BaseModels;
using Library.Models.DTO;

namespace Library.Models.ViewModels
{
    public class SearchViewModel
    {
        public string searchCategory { get; set; } = "";
        public string inputValue { get; set; } = "";
        public IQueryable<BookDTO> Books { get; set; } = Enumerable.Empty<BookDTO>().AsQueryable();
        public IQueryable<BlogPost> BlogPosts { get; set; } = Enumerable.Empty<BlogPost>().AsQueryable();
        public IQueryable<SubjectDTO> Subjects{ get; set; } = Enumerable.Empty<SubjectDTO>().AsQueryable();
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }
}
