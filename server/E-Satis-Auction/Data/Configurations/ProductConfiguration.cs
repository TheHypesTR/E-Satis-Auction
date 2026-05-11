using E_Satis_Auction.Models.Categories;
using E_Satis_Auction.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    private static readonly ValueComparer<Dictionary<string, string>> BaseAttributesComparer = new(
        (left, right) => DictionariesEqual(left, right),
        dictionary => GetDictionaryHashCode(dictionary),
        dictionary => dictionary.ToDictionary(entry => entry.Key, entry => entry.Value));

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.Property(product => product.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(product => product.Sku)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(product => product.Barcode)
            .HasMaxLength(64);

        builder.Property(product => product.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(product => product.UnitOfMeasure)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique();

        builder.HasIndex(product => product.Barcode);
        builder.HasIndex(product => product.CategoryId);
        builder.HasIndex(product => product.IsActive);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property<Dictionary<string, string>>("_baseAttributes")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("BaseAttributes")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .Metadata.SetValueComparer(BaseAttributesComparer);

        builder.HasIndex("_baseAttributes")
            .HasMethod("gin");

        builder.Ignore(product => product.BaseAttributes);
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