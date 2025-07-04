using MediatR;
using Microsoft.EntityFrameworkCore;
using ArggonStores.Api.Data;
using ArggonStores.Api.Models;

namespace ArggonStores.Api.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<GetProductByIdQueryResult>
{
    public int Id { get; set; }
}

public class GetProductByIdQueryResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public ProductResponse? Product { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdQueryResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        ApplicationDbContext context,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _context.Products
                .Where(p => p.Id == request.Id)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    Size = p.Size,
                    Color = p.Color,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Brand = p.Brand,
                    Material = p.Material,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return new GetProductByIdQueryResult
                {
                    Success = false,
                    ErrorMessage = "Product not found"
                };
            }

            return new GetProductByIdQueryResult
            {
                Success = true,
                Product = product
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", request.Id);
            return new GetProductByIdQueryResult
            {
                Success = false,
                ErrorMessage = "Failed to retrieve product"
            };
        }
    }
}