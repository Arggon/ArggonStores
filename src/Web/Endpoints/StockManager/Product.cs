using ArggonStores.Application.Common.Models;
using ArggonStores.Application.StockManager.Products.Commands.Create;
using ArggonStores.Application.StockManager.Products.Commands.Delete;
using ArggonStores.Application.StockManager.Products.Commands.Patch;
using ArggonStores.Application.StockManager.Products.Commands.Update;
using ArggonStores.Application.StockManager.Products.Queries.Get;
using ArggonStores.Application.StockManager.Products.Queries.GetPaginatedList;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ArggonStores.Web.Endpoints.StockManager;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateProduct)
            .MapGet(GetProducts)
            .MapGet(GetProduct, "{id}")
            .MapPut(UpdateProduct, "{id}")
            .MapPatch(PatchProduct, "{id}")
            .MapDelete(DeleteProduct, "{id}")
            ;
    }

    private async Task<Created<int>> CreateProduct(ISender sender, CreateProductCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(ProductDto)}/{id}", id);
    }

    private async Task<Ok<PaginatedList<ProductDto>>> GetProducts(ISender sender, [AsParameters] GetProductsPaginatedListQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result);
    }

    private async Task<Ok<ProductDto>> GetProduct(ISender sender, int id)
    {
        var query = new GetProductQuery { Id = id };
        var item = await sender.Send(query);

        return TypedResults.Ok(item);
    }

    private async Task<bool> UpdateProduct(ISender sender, int id, UpdateProductCommand command)
    {
        command.Id = id;
        return await sender.Send(command);
    }

    private async Task<bool> PatchProduct (ISender sender, int id, PatchProductCommand command)
    {
        command.Id = id;
        return await sender.Send(command);
    }
    private async Task<bool> DeleteProduct(ISender sender, int id)
    {
        var command = new DeleteProductCommand() { Id = id };
        return await sender.Send(command);
    }
}
