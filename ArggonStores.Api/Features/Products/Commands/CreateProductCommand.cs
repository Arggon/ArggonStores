using MediatR;
using Microsoft.EntityFrameworkCore;
using ArggonStores.Api.Data;
using ArggonStores.Api.Models;

namespace ArggonStores.Api.Features.Products.Commands;

public class CreateProductCommand : IRequest<CreateProductCommandResult>
{
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
}

public class CreateProductCommandResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public ProductResponse? Product { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductCommandResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        ApplicationDbContext context,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CreateProductCommandResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Size = request.Size,
                Color = request.Color,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                Brand = request.Brand,
                Material = request.Material,
                ImageUrl = request.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateProductCommandResult
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
            _logger.LogError(ex, "Error creating product");
            return new CreateProductCommandResult
            {
                Success = false,
                ErrorMessage = "Failed to create product"
            };
        }
    }
}