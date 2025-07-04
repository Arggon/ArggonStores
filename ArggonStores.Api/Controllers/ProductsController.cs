using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ArggonStores.Api.Models;
using ArggonStores.Api.Features.Products.Commands;
using ArggonStores.Api.Features.Products.Queries;

namespace ArggonStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? category = null, [FromQuery] bool? isActive = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsQuery
        {
            Category = category,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return StatusCode(500, new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            products = result.Products,
            totalCount = result.TotalCount,
            pageNumber = result.PageNumber,
            pageSize = result.PageSize,
            totalPages = result.TotalPages
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            if (result.ErrorMessage == "Product not found")
            {
                return NotFound();
            }
            return StatusCode(500, new { message = result.ErrorMessage });
        }

        return Ok(result.Product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Size = request.Size,
            Color = request.Color,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Brand = request.Brand,
            Material = request.Material,
            ImageUrl = request.ImageUrl
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return StatusCode(500, new { message = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetProduct), new { id = result.Product!.Id }, result.Product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "Product ID mismatch" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new UpdateProductCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Size = request.Size,
            Color = request.Color,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Brand = request.Brand,
            Material = request.Material,
            ImageUrl = request.ImageUrl,
            IsActive = request.IsActive
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            if (result.ErrorMessage == "Product not found")
            {
                return NotFound();
            }
            return StatusCode(500, new { message = result.ErrorMessage });
        }

        return Ok(result.Product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            if (result.ErrorMessage == "Product not found")
            {
                return NotFound();
            }
            return StatusCode(500, new { message = result.ErrorMessage });
        }

        return NoContent();
    }
}