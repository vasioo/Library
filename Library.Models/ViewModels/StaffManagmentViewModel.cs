using Library.DataAccess.MainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.ViewModels
{
    public class StaffManagmentViewModel
    {
        public IQueryable<ApplicationUser> Users{ get; set; }
    }
}
