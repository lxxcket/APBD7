namespace APBD7.Service;

public interface IWarehouseService
{
    int AddProduct(OrderRequest orderRequest);
    int AddProductUsingProcedure(OrderRequest orderRequest);
}