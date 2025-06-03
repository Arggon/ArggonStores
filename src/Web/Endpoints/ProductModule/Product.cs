using ArggonStores.Application.Common.Models;
using ArggonStores.Application.ProductModule.Products.Commands.Create;
using ArggonStores.Application.ProductModule.Products.Commands.Delete;
using ArggonStores.Application.ProductModule.Products.Commands.Patch;
using ArggonStores.Application.ProductModule.Products.Commands.Update;
using ArggonStores.Application.ProductModule.Products.Queries.GetById;
using ArggonStores.Application.ProductModule.Products.Queries.GetPaginatedList;
using ArggonStores.Application.ProductModule.Products.Queries.GetProductByFilter;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ArggonStores.Web.Endpoints.ProductModule;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateProduct)
            .MapGet(GetProducts)
            .MapGet(GetProduct, "{id}")
            .MapGet(GetProductsByFilters, "GetProductsByFilters")
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
    private async Task<Ok<ProductDto>> GetProduct(ISender sender, int id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var item = await sender.Send(query);

        return TypedResults.Ok(item);
    }

    private async Task<Ok<PaginatedList<ProductDto>>> GetProducts(ISender sender, [AsParameters] GetProductsPaginatedListQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result);
    }

    private async Task<Ok<PaginatedList<ProductDto>>> GetProductsByFilters(ISender sender, [AsParameters] GetProductByFiltersQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result);
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
