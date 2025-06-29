using Microsoft.EntityFrameworkCore;
using WebStore.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация User
            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.Id);
                user.HasIndex(u => u.Email).IsUnique();

                user.Property(u => u.Email).IsRequired().HasMaxLength(255);
                user.Property(u => u.PasswordHash).IsRequired();
                user.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                user.Property(u => u.CreatedAt).IsRequired();

                // Исправленная конфигурация для UserRole
                user.Property(u => u.Role)
                    .HasConversion<string>()
                    .HasDefaultValue(UserRole.User)
                    .HasSentinel(UserRole.User)
                    .IsRequired();

                user.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                user.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                user.HasOne(u => u.Cart)
                    .WithOne(c => c.User)
                    .HasForeignKey<Cart>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация Product
            modelBuilder.Entity<Product>(product =>
            {
                product.HasKey(p => p.Id);
                product.Property(p => p.Name).IsRequired().HasMaxLength(100);
                product.Property(p => p.Description).HasMaxLength(500);
                product.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();

                product.HasMany(p => p.Variants)
                    .WithOne(pv => pv.Product)
                    .HasForeignKey(pv => pv.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                product.HasMany(p => p.Reviews)
                    .WithOne(r => r.Product)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                product.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(category =>
            {
                category.HasKey(c => c.Id);
                category.Property(c => c.Name).IsRequired().HasMaxLength(100);
                category.Property(c => c.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<ProductVariant>(variant =>
            {
                variant.HasKey(pv => pv.Id);
                variant.Property(pv => pv.Size).IsRequired().HasMaxLength(50);
                variant.Property(pv => pv.Color).IsRequired().HasMaxLength(50);
                variant.Property(pv => pv.StockQuantity).IsRequired();
                variant.Property(pv => pv.AdditionalPrice).HasColumnType("decimal(18,2)").IsRequired();

                variant.HasMany(pv => pv.CartItems)
                    .WithOne(ci => ci.ProductVariant)
                    .HasForeignKey(ci => ci.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                variant.HasMany(pv => pv.OrderItems)
                    .WithOne(oi => oi.ProductVariant)
                    .HasForeignKey(oi => oi.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Cart>(cart =>
            {
                cart.HasKey(c => c.Id);
                cart.HasOne(c => c.User)
                    .WithOne(u => u.Cart)
                    .HasForeignKey<Cart>(c => c.UserId);

                cart.HasMany(c => c.CartItems)
                    .WithOne(ci => ci.Cart)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(cartItem =>
            {
                cartItem.HasKey(ci => ci.Id);
                cartItem.Property(ci => ci.Quantity).IsRequired();
            });

            modelBuilder.Entity<Order>(order =>
            {
                order.HasKey(o => o.Id);
                order.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                order.Property(o => o.Status).IsRequired().HasDefaultValue(OrderStatus.Pending);
                order.Property(o => o.CreatedAt).IsRequired();
                order.Property(o => o.UpdatedAt).IsRequired();

                order.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(orderItem =>
            {
                orderItem.HasKey(oi => oi.Id);
                orderItem.Property(oi => oi.Quantity).IsRequired();
                orderItem.Property(oi => oi.PriceAtPurchase).HasColumnType("decimal(18,2)").IsRequired();
            });

            modelBuilder.Entity<Review>(review =>
            {
                review.HasKey(r => r.Id);
                review.Property(r => r.Rating).IsRequired();
                review.Property(r => r.Comment).HasMaxLength(1000);
                review.Property(r => r.CreatedAt).IsRequired();

                review.Property(r => r.Rating)
                    .HasConversion(
                        v => (int)v,
                        v => v)
                    .HasAnnotation("Range", new[] { 1, 5 });
            });
        }
    }
}