using E_Satis_Auction.Models.Categories;
using E_Satis_Auction.Models.Facilities;
using E_Satis_Auction.Models.Items;
using E_Satis_Auction.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    private static readonly ValueComparer<Dictionary<string, string>> DynamicAttributesComparer = new(
        (left, right) => DictionariesEqual(left, right),
        dictionary => GetDictionaryHashCode(dictionary),
        dictionary => dictionary.ToDictionary(entry => entry.Key, entry => entry.Value));

    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.Property(item => item.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(item => item.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(item => item.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(item => item.Mode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(item => item.UnitOfMeasure)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(item => new { item.Status, item.Mode });
        builder.HasIndex(item => item.ProductId);
        builder.HasIndex(item => item.CategoryId);
        builder.HasIndex(item => item.FacilityId);
        builder.HasIndex(item => item.SourceItemId);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(item => item.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(item => item.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(item => item.SourceItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property<Dictionary<string, string>>("_dynamicAttributes")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("DynamicAttributes")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .Metadata.SetValueComparer(DynamicAttributesComparer);

        builder.HasIndex("_dynamicAttributes")
            .HasMethod("gin");

        builder.Ignore(item => item.DynamicAttributes);
    }

    private static bool DictionariesEqual(Dictionary<string, string>? left, Dictionary<string, string>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null || left.Count != right.Count)
        {
            return false;
        }

        return left.All(pair => right.TryGetValue(pair.Key, out string? value) && value == pair.Value);
    }

    private static int GetDictionaryHashCode(Dictionary<string, string> dictionary)
    {
        HashCode hash = new();
        foreach ((string key, string value) in dictionary.OrderBy(entry => entry.Key, StringComparer.Ordinal))
        {
            hash.Add(key, StringComparer.Ordinal);
            hash.Add(value, StringComparer.Ordinal);
        }

        return hash.ToHashCode();
    }
}
