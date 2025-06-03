using System.Text.Json.Serialization;
using ArggonStores.Application.Common.Interfaces;

namespace ArggonStores.Application.ProductModule.Products.Commands.Patch;

public record PatchProductCommand : IRequest<bool>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string? InternalCode { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public int? Stock { get; set; }
    public decimal? PurchasePrice { get; set; }
}

public class PatchCommandHandler(IApplicationDbContext context) : IRequestHandler<PatchProductCommand, bool>
{
    public async Task<bool> Handle(PatchProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Product.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (product == null) return false;

        if (request.InternalCode != null) product.InternalCode = request.InternalCode;
        if (request.Code != null) product.Code = request.Code;
        if (request.Name != null) product.Name = request.Name;
        if (request.Description != null) product.Description = request.Description;
        if (request.ImageUrl != null) product.ImageUrl = request.ImageUrl;
        if (request.Brand != null) product.Brand = request.Brand;
        if (request.Category != null) product.Category = request.Category;
        if (request.Stock.HasValue) product.Stock = request.Stock.Value;
        if (request.PurchasePrice.HasValue) product.PurchasePrice = request.PurchasePrice.Value;

        context.Product.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
