namespace APBD7.Repository;

public interface IWarehouseRepository
{
    Task<int> AddProductToWarehouseAsync(OrderRequest orderRequest);
    Task<int> ProductExistsAsync(int id);
    Task<int> WarehouseExistsAsync(int id);
    Task<int> PurchaseOrderExistsAsync(int productId, int amount, DateTime createdAt);
    Task<int> OrderFulfilledAsync(int idProduct);

    Task<int> AddProductProcedureAsync(OrderRequest orderRequest);
}