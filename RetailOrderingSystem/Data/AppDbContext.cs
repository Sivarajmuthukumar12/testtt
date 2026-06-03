/*
 * Folder: Data
 * File: AppDbContext.cs
 * Purpose: The EF Core DbContext — the bridge between C# models and SQL Server tables.
 *          Every DbSet<T> property becomes a table in the database.
 * Who Calls It: Repositories, DataSeeder, Program.cs (for migrations)
 * Flow: Repository → AppDbContext → EF Core → SQL Server
 * Interview Tip: DbContext is the Unit of Work. SaveChangesAsync() commits all pending changes
 *                in a single transaction.
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor — receives options (connection string) via Dependency Injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Each DbSet maps to a SQL Server table
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<LoyaltyPoint> LoyaltyPoints { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for money fields
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.FinalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountPercentage)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Coupon>()
                .Property(c => c.FixedDiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Coupon>()
                .Property(c => c.MinimumOrderAmount)
                .HasColumnType("decimal(18,2)");

            // Unique email constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Unique coupon code constraint
            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();

            // One User → One Cart (One-To-One)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId);

            // One User → One LoyaltyPoint (One-To-One)
            modelBuilder.Entity<LoyaltyPoint>()
                .HasOne(lp => lp.User)
                .WithOne(u => u.LoyaltyPoint)
                .HasForeignKey<LoyaltyPoint>(lp => lp.UserId);

            // Order → DeliveryPartner (separate FK, same User table)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryPartner)
                .WithMany()
                .HasForeignKey(o => o.DeliveryPartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → Customer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
