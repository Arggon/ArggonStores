using ArggonStores.Domain.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArggonStores.Infrastructure.Data.Configurations.ProductModule;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(i => i.InternalCode)
            .HasMaxLength(50);

        builder.Property(i => i.Code)
            .HasMaxLength(50);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.ImageUrl)
            .HasMaxLength(500);

        builder.Property(i => i.Brand)
            .HasMaxLength(100);

        builder.Property(i => i.Category)
            .HasMaxLength(100);

        builder.Property(i => i.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.Stock)
            .IsRequired();
    }
}
