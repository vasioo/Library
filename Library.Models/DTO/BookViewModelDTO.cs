namespace Library.Models.DTO
{
    public class BookViewModelDTO
    {
        public string ISBN { get; set; } = "";
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Authors { get; set; } = "";
        public DateTime PublishDate { get; set; }
        public string Category { get; set; } = "";
        public string Language { get; set; } = "";
    }

}
