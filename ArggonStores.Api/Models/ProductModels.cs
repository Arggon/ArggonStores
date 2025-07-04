using System.ComponentModel.DataAnnotations;

namespace ArggonStores.Api.Models;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string Category { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Size is required")]
    [StringLength(50, ErrorMessage = "Size cannot exceed 50 characters")]
    public string Size { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Color is required")]
    [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
    public string Color { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }
    
    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string Brand { get; set; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Material cannot exceed 100 characters")]
    public string Material { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string ImageUrl { get; set; } = string.Empty;
}

public class UpdateProductRequest
{
    [Required(ErrorMessage = "Product ID is required")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string Category { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Size is required")]
    [StringLength(50, ErrorMessage = "Size cannot exceed 50 characters")]
    public string Size { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Color is required")]
    [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
    public string Color { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }
    
    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string Brand { get; set; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Material cannot exceed 100 characters")]
    public string Material { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string ImageUrl { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}