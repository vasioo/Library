using Library.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.ViewModels
{
    public class LeasedTrackerViewModel
    {
        public IQueryable<UserLeasedBookMappingTable> LeasedBooks{ get; set; }
        public string Category { get; set; } = "";
    }
}
