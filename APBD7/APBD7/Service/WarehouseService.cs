using APBD7.Repository;

namespace APBD7.Service;

public class WarehouseService : IWarehouseService
{
    private IWarehouseRepository _repository;

    public WarehouseService(IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddProduct(OrderRequest orderRequest)
    {

        if (await _repository.ProductExistsAsync(orderRequest.IdProduct) <= 0 ||
            await _repository.WarehouseExistsAsync(orderRequest.IdWarehouse) <= 0 || orderRequest.Amount <= 0)
            throw new Exception("Invalid product, warehouse or amount");

        int idOrder = await _repository.PurchaseOrderExistsAsync(orderRequest.IdProduct, orderRequest.Amount, DateTime.Now);

        if (await _repository.OrderFulfilledAsync(idOrder) >= 1)
            throw new Exception("Order was already fulfilled");
        
        
        return await _repository.AddProductToWarehouseAsync(orderRequest);
    }

    public async Task<int> AddProductUsingProcedure(OrderRequest orderRequest)
    {
        return await _repository.AddProductProcedureAsync(orderRequest);
    }
}