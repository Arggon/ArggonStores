using MediatR;
using Microsoft.EntityFrameworkCore;
using ArggonStores.Api.Data;

namespace ArggonStores.Api.Features.Products.Commands;

public class DeleteProductCommand : IRequest<DeleteProductCommandResult>
{
    public int Id { get; set; }
}

public class DeleteProductCommandResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductCommandResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        ApplicationDbContext context,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DeleteProductCommandResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
            {
                return new DeleteProductCommandResult
                {
                    Success = false,
                    ErrorMessage = "Product not found"
                };
            }

            // Soft delete by setting IsActive to false
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new DeleteProductCommandResult
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}", request.Id);
            return new DeleteProductCommandResult
            {
                Success = false,
                ErrorMessage = "Failed to delete product"
            };
        }
    }
}