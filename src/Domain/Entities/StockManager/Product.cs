namespace ArggonStores.Domain.Entities.StockManager;

public class Product : BaseAuditableEntity
{
    public string? InternalCode { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
    public decimal PurchasePrice { get; set; }
}
