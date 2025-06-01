using ArggonStores.Application.Common.Interfaces;
using ArggonStores.Application.Common.Mappings;
using ArggonStores.Application.Common.Models;
using ArggonStores.Application.StockManager.Products.Queries.Get;

namespace ArggonStores.Application.StockManager.Products.Queries.GetPaginatedList;

public record GetProductsPaginatedListQuery : IRequest<PaginatedList<ProductDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetProductsPaginatedListQuery, PaginatedList<ProductDto>>
{
    public async Task<PaginatedList<ProductDto>> Handle(GetProductsPaginatedListQuery request, CancellationToken cancellationToken)
    {
        return await context.Product
            .OrderBy(x => x.Name)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
