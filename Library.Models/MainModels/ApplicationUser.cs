using Library.Models.BaseModels;
using Library.Models.UserModels.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Library.DataAccess.MainModels
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public int Points { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Position { get; set; } = "";
        public decimal Salary { get; set; }
        public DateTime StartOfMembership { get; set; } = DateTime.Now;
        public string BanStatus { get; set; } = "";
    }
}
