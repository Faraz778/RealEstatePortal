
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Models;

namespace RealEstatePortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
      
        public DbSet<ListingImage> ListingImages { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }



        public DbSet<Settings> Settings { get; set; }
        public DbSet<Package> Packages { get; set; }



        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaypalSetting> PaypalSettings { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<Listing>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
        }
    }
}

