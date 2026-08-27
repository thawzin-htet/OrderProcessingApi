using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Models;

namespace OrderProcessingApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.RowVersion).IsRowVersion();
            base.OnModelCreating(modelBuilder);
        }
    }
}
