namespace APBD7.Service;

public interface IWarehouseService
{
    Task<int> AddProduct(OrderRequest orderRequest);
    Task<int> AddProductUsingProcedure(OrderRequest orderRequest);
}