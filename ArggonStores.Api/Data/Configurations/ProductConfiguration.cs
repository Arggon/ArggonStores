using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArggonStores.Api.Models;

namespace ArggonStores.Api.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.Description)
            .HasMaxLength(1000);
            
        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.Size)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(p => p.Color)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
            
        builder.Property(p => p.StockQuantity)
            .IsRequired();
            
        builder.Property(p => p.Brand)
            .HasMaxLength(100);
            
        builder.Property(p => p.Material)
            .HasMaxLength(100);
            
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);
            
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Index for common queries
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => new { p.Category, p.IsActive });
    }
}