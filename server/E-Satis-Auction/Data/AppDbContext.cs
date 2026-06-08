using System.Linq.Expressions;
using E_Satis_Auction.Common.Entities.Interfaces;
using E_Satis_Auction.Models.Categories;
using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Common;
using E_Satis_Auction.Models.Dispatches;
using E_Satis_Auction.Models.Facilities;
using E_Satis_Auction.Models.InventoryTransactions;
using E_Satis_Auction.Models.Items;
using E_Satis_Auction.Models.Products;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace E_Satis_Auction.Data;

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
    public DbSet<ProductListing> ProductListings { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignProduct> CampaignProducts { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<PaymentAttempt> PaymentAttempts { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    public DbSet<PurchaseOrderLineAllocation> PurchaseOrderLineAllocations { get; set; }
    public DbSet<Auction> Auctions { get; set; }
    public DbSet<AuctionBid> AuctionBids { get; set; }
    public DbSet<AuctionInventoryReservation> AuctionInventoryReservations { get; set; }
    public DbSet<ReturnRequest> ReturnRequests { get; set; }
    public DbSet<ReturnRequestLine> ReturnRequestLines { get; set; }
    public DbSet<UserSaleRequest> UserSaleRequests { get; set; }
    public DbSet<PartSaleOperation> PartSaleOperations { get; set; }

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
