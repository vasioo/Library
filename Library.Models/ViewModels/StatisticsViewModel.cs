using Library.Models.BaseModels;
using Library.Models.DTO;

namespace Library.Models.ViewModels
{
    public class StatisticsViewModel
    {
        public int AmountOfUsers { get; set; }
        public int AmountOfWorkers { get; set; }
        public ReportBookDTO MostLeasedBook { get; set; } = new ReportBookDTO();
        public List<string> MostReadGenres { get; set; } = new List<string>();
    }
}
