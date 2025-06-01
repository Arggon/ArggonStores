using ArggonStores.Application.Common.Interfaces;
using ArggonStores.Domain.Entities.StockManager;

namespace ArggonStores.Application.StockManager.Products.Commands.Create;

public record CreateProductCommand : IRequest<int>
{
    public string? InternalCode { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
    public decimal PurchasePrice { get; set; }
}

public class CreateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = new Product
        {
            InternalCode = request.InternalCode,
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Brand = request.Brand,
            Category = request.Category,
            Stock = request.Stock,
            PurchasePrice = request.PurchasePrice,
        };

        context.Product.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
