using ArggonStores.Application.Common.Interfaces;

namespace ArggonStores.Application.ProductModule.Products.Commands.Delete;

public record DeleteProductCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Product.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            return false;
        }
        context.Product.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
