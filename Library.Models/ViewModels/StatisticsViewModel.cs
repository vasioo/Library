using Library.Models.BaseModels;

namespace Library.Models.ViewModels
{
    public class StatisticsViewModel
    {
        public int AmountOfUsers { get; set; }
        public int AmountOfWorkers { get; set; }

        public Book MostLeasedBook { get; set; } = new Book();

        public List<string> MostReadGenres { get; set; } = new List<string>();
    }
}
