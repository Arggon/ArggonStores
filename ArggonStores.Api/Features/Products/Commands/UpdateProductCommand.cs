using MediatR;
using Microsoft.EntityFrameworkCore;
using ArggonStores.Api.Data;
using ArggonStores.Api.Models;

namespace ArggonStores.Api.Features.Products.Commands;

public class UpdateProductCommand : IRequest<UpdateProductCommandResult>
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
    public bool IsActive { get; set; }
}

public class UpdateProductCommandResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public ProductResponse? Product { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductCommandResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        ApplicationDbContext context,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UpdateProductCommandResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
            {
                return new UpdateProductCommandResult
                {
                    Success = false,
                    ErrorMessage = "Product not found"
                };
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Category = request.Category;
            product.Size = request.Size;
            product.Color = request.Color;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.Brand = request.Brand;
            product.Material = request.Material;
            product.ImageUrl = request.ImageUrl;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateProductCommandResult
            {
                Success = true,
                Product = new ProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Category = product.Category,
                    Size = product.Size,
                    Color = product.Color,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Brand = product.Brand,
                    Material = product.Material,
                    ImageUrl = product.ImageUrl,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    IsActive = product.IsActive
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", request.Id);
            return new UpdateProductCommandResult
            {
                Success = false,
                ErrorMessage = "Failed to update product"
            };
        }
    }
}