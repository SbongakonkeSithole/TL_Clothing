using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TL_Clothing.Models;
using static NuGet.Packaging.PackagingConstants;

namespace TL_Clothing.Data
{
    public class SqlDbContext : IdentityDbContext<Customer>
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options):base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasOne<Product>(a => a.Product)
                .WithMany(y => y.OrderItems)
                .HasForeignKey(z => z.ProductId);

            modelBuilder.Entity<OrderItem>()
               .HasOne<Order>(a => a.Order)
               .WithMany(y => y.OrderItems)
               .HasForeignKey(z => z.OrderId);

            modelBuilder.Entity<CartItem>()
              .HasOne<Cart>(a => a.Cart)
              .WithMany(y => y.CartItem)
              .HasForeignKey(z => z.CartId);

            modelBuilder.Entity<CartItem>()
              .HasOne<Product>(a => a.Product)
              .WithMany(y => y.CartItems)
              .HasForeignKey(z => z.ProductId);
        }
    }
}
