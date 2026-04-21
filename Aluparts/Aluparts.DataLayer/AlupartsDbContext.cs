using Microsoft.EntityFrameworkCore;
using Aluparts.DataLayer.Entities;

namespace Aluparts.DataLayer;

public class AlupartsDbContext : DbContext
{
    public AlupartsDbContext(DbContextOptions<AlupartsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ClientOrder> ClientOrders { get; set; }
    public DbSet<SupplierProduct> SupplierProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}