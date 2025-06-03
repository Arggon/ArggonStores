using System.Text.Json.Serialization;
using ArggonStores.Application.Common.Interfaces;

namespace ArggonStores.Application.ProductModule.Products.Commands.Update;

public record UpdateProductCommand : IRequest<bool>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string InternalCode => string.Empty;
    public string Code => string.Empty;
    public string Name => string.Empty;
    public string Description => string.Empty;
    public string ImageUrl => string.Empty;
    public string Brand => string.Empty;
    public string Category => string.Empty;
    public int Stock => 0;
    public decimal PurchasePrice => 0.0m;
}

public class UpdateCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Product.FirstOrDefaultAsync(x => x.Id == request.Id,
            cancellationToken);
        if (product == null) return false;
        product.InternalCode = request.InternalCode;
        product.Code = request.Code;
        product.Name = request.Name;
        product.Description = request.Description;
        product.ImageUrl = request.ImageUrl;
        product.Brand = request.Brand;
        product.Category = request.Category;
        product.Stock = request.Stock;
        product.PurchasePrice = request.PurchasePrice;
        context.Product.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
