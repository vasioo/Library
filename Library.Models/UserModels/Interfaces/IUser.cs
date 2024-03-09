namespace Library.Models.UserModels.Interfaces
{
    public interface IUser
    {
        public int Points { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
