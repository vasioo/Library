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
        public DbSet<UserLeasedBookMappingTable> UserLeasedBooks { get; set; }
        public DbSet<BookCategory> Categories { get; set; }
        public DbSet<LibrarianReport> Reports { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<BookSubject> BookSubjects { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }

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
            modelBuilder.Entity<FavouriteBooks>()
                .HasKey(fb => new { fb.UserId, fb.BookId });
            modelBuilder.Entity<FavouriteBooks>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(fb => fb.UserId);

            modelBuilder.Entity<FavouriteBooks>()
                .HasOne<Book>()
                .WithMany()
                .HasForeignKey(fb => fb.BookId);

            modelBuilder.Entity<Book>().Navigation(e => e.Genre).AutoInclude();
            modelBuilder.Entity<BookSubject>().Navigation(e => e.BookCategories).AutoInclude();

            modelBuilder.Entity<BookCategory>()
            .HasIndex(ci => new { ci.CategoryName })
            .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
