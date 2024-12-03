using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCraft_Inventory.Models;

namespace MyCraft_Inventory.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Custom configurations (if needed)
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.IsEmployee).HasDefaultValue(false);
        });

        builder.Entity<ApplicationUser>(entity => 
        {
            entity.Property(e => e.EmployeeCode).HasDefaultValue("");
        });

        builder.Entity<ProductViewModel>()
        .Property(e => e.ID)
        .ValueGeneratedOnAdd(); // Explicitly specifies auto-increment

        builder.Entity<CartItemViewModel>()
        .HasKey(c => new { c.UserId, c.Name });

        builder.Entity<TransactionObjectModel>()
        .Property(e => e.ID)
        .ValueGeneratedOnAdd();
    }

    public DbSet<ProductViewModel> Products { get; set; } = default!;

    public DbSet<CartItemViewModel> CartItems { get; set; } = default!;

    public DbSet<TransactionObjectModel> TransactionHistory { get; set; } = default!;
}
