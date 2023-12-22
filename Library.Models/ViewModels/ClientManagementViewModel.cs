using Library.DataAccess.MainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.ViewModels
{
    public class ClientManagementViewModel
    {
        public IQueryable<ApplicationUser> Users{ get; set; }
    }
}
