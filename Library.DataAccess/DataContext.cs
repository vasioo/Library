using Library.DataAccess.MainModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        private bool seedDb;
        public DataContext(DbContextOptions<DataContext> options, bool seedDb = true)
            : base(options)
        {
            this.seedDb = seedDb;
        }
    }
}
