using ArggonStores.Application.Common.Interfaces;
using ArggonStores.Domain.Entities.StockManager;

namespace ArggonStores.Application.StockManager.Products.Queries.Get;

public record GetProductQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}

public class GetProductQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetProductQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var item = await context.Product
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        return mapper.Map<ProductDto>(item);
    }
}

public class ProductDto
{
    public int Id { get; set; }
    public string? InternalCode { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>();
        }
    }
}
