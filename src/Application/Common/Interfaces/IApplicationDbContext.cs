using ArggonStores.Domain.Entities;
using ArggonStores.Domain.Entities.ProductModule;

namespace ArggonStores.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<Product> Product { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
