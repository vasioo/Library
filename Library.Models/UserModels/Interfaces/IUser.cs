using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.UserModels.Interfaces
{
    public interface IUser
    {
        public String? SecondaryPhone { get; set; }
        public String Street { get; set; }
        public String City { get; set; }
        public String PostalCode { get; set; }
        public String District { get; set; }
        public String Province { get; set; }
        public String Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        //add more for books
    }
}
