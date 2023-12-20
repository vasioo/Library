using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<Book> Books{ get; set; }
        public DbSet<FavouriteBooks> FavouriteBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (this.seedDb)
            {
                modelBuilder.Entity<IdentityRole>().HasData(
                     new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                     new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" },
                     new IdentityRole { Id = "3", Name = "Worker", NormalizedName = "WORKER" }
                 );
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
