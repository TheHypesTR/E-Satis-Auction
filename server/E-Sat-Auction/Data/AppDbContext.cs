using System.Linq.Expressions;
using e_Sat_Auction.Common.Entities.Interfaces;
using e_Sat_Auction.Models.Categories;
using e_Sat_Auction.Models.Common;
using e_Sat_Auction.Models.Dispatches;
using e_Sat_Auction.Models.Facilities;
using e_Sat_Auction.Models.InventoryTransactions;
using e_Sat_Auction.Models.Items;
using e_Sat_Auction.Models.Products;
using e_Sat_Auction.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace e_Sat_Auction.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<FacilityManager> FacilityManagers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
    public DbSet<CategoryAttributeOption> CategoryAttributeOptions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Dispatch> Dispatches { get; set; }
    public DbSet<DispatchLineItem> DispatchLineItems { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplySoftDeleteQueryFilter(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry is { Entity: ISoftDeletable softDeletableEntity, State: EntityState.Deleted })
            {
                entry.State = EntityState.Modified;
                softDeletableEntity.Delete();
            }

            if (entry.Entity is IAuditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        DateTime dateNow = DateTime.UtcNow;
                        entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = dateNow;
                        entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = dateNow;
                        break;

                    case EntityState.Modified:
                        entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                        break;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                ParameterExpression parameter = Expression.Parameter(entityType.ClrType, "e");
                MemberExpression property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                BinaryExpression condition = Expression.Equal(property, Expression.Constant(false));
                LambdaExpression lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}