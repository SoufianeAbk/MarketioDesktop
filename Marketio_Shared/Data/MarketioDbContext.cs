using Marketio_Shared.Entities;
using Marketio_Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Marketio_Shared.Data
{
    public class MarketioDbContext : IdentityDbContext<AppUser>
    {
        public MarketioDbContext(DbContextOptions<MarketioDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductTranslation> ProductTranslations => Set<ProductTranslation>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Vereist voor IdentityDbContext: configureert de Identity-tabellen.
            base.OnModelCreating(modelBuilder);

            // Decimal-precisie expliciet zetten (voorkomt EF-truncatiewaarschuwing).
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.TotalPrice)
                .HasPrecision(18, 2);
        }
    }
}