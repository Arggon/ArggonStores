using System.ComponentModel.DataAnnotations;

namespace ArggonStores.Api.Models;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty; // T-shirt, Jeans, Dress, etc.
    
    [Required]
    [MaxLength(50)]
    public string Size { get; set; } = string.Empty; // S, M, L, XL, etc.
    
    [Required]
    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }
    
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Material { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
}