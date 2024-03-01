using Library.Models.BaseModels;
using Library.Models.UserModels.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Library.DataAccess.MainModels
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public int Points { get; set; }
        public string? SecondaryPhone { get; set; } = "";
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string District { get; set; } = "";
        public string Province { get; set; } = "";
        public string Country { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime BirthDate { get; set; }=DateTime.Now;
        public string Position { get; set; } = "";
        public decimal Salary { get; set; }
        public DateTime StartOfMembership { get; set; } = DateTime.Now;
        public string Membership { get; set; } = "";
        public ICollection<FavouriteBooks> FavouriteBooks { get; set; }
    }
}
