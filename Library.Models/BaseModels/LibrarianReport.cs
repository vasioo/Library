using Library.Models.UserModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.BaseModels
{
    public class LibrarianReport:IEntity
    {
        public int Id { get; set; }

        public string Reason { get; set; } = "";

        public string Complaint { get; set; } = "";

        public DateTime TimeOfCreation { get; set; } = DateTime.Now;
    }
}
