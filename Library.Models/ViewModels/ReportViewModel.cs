using Library.Models.BaseModels;
using Library.Models.DTO;

namespace Library.Models.ViewModels
{
    public class ReportViewModel
    {
        public int AmountOfUsers { get; set; }
        public int AmountOfBooks { get; set; }
        public int AmountOfCategories { get; set; }
        public int AmountOfSubjects { get; set; }
        public int AmountOfLeased { get; set; }
        public ReportBookDTO MostLeasedBook { get; set; } = new ReportBookDTO();
        public List<string> MostReadGenres { get; set; } = new List<string>();
    }
}
