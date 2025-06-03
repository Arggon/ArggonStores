using ArggonStores.Application.Common.Interfaces;
using ArggonStores.Application.Common.Models;
using ArggonStores.Application.ProductModule.Products.Queries.GetById;

namespace ArggonStores.Application.ProductModule.Products.Queries.GetProductByFilter;

public record GetProductByFiltersQuery : IRequest<PaginatedList<ProductDto>>
{
    public string? InternalCode { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public int? Stock { get; set; }
    public decimal? PurchasePrice { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetProductByFiltersQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetProductByFiltersQuery, PaginatedList<ProductDto>>
{
    public async Task<PaginatedList<ProductDto>> Handle(GetProductByFiltersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Product.AsQueryable();
        if (!string.IsNullOrEmpty(request.InternalCode))
            query = query.Where(p => p.InternalCode == request.InternalCode);

        if (!string.IsNullOrEmpty(request.Code))
            query = query.Where(p => p.Code == request.Code);

        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(p => p.Name != null && p.Name.Contains(request.Name));

        if (!string.IsNullOrEmpty(request.Description))
            query = query.Where(p => p.Description != null && p.Description.Contains(request.Description));

        if (!string.IsNullOrEmpty(request.ImageUrl))
            query = query.Where(p => p.ImageUrl == request.ImageUrl);

        if (!string.IsNullOrEmpty(request.Brand))
            query = query.Where(p => p.Brand == request.Brand);

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(p => p.Category == request.Category);

        if (request.Stock != null)
            query = query.Where(p => p.Stock >= request.Stock);

        if (request.PurchasePrice != null)
            query = query.Where(p => p.PurchasePrice >= request.PurchasePrice);

        return await PaginatedList<ProductDto>.CreateAsync(
            query.ProjectTo<ProductDto>(mapper.ConfigurationProvider),
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
