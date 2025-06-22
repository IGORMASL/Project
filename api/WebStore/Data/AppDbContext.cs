using Microsoft.EntityFrameworkCore;
using WebStore.Models;

namespace WebStore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price)
                  .HasColumnType("decimal(18,2)");

            entity.Property(p => p.Name)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(p => p.Description)
                  .HasMaxLength(500);
        });
    }
}