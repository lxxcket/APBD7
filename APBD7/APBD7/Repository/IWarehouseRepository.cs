namespace APBD7.Repository;

public interface IWarehouseRepository
{
    int AddProductToWarehouse(OrderRequest orderRequest);
    int ProductExists(int id);
    int WarehouseExists(int id);
    int PurchaseOrderExists(int productId, int amount, DateTime createdAt);
    int OrderFulfilled(int idProduct);
}