using APBD7.Repository;

namespace APBD7.Service;

public class WarehouseService : IWarehouseService
{
    private IWarehouseRepository _repository;

    public WarehouseService(IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public int AddProduct(OrderRequest orderRequest)
    {

        if (_repository.ProductExists(orderRequest.IdProduct) <= 0 ||
            _repository.WarehouseExists(orderRequest.IdWarehouse) <= 0 || orderRequest.Amount <= 0)
            throw new Exception("Invalid product, warehouse or amount");

        int idOrder = _repository.PurchaseOrderExists(orderRequest.IdProduct, orderRequest.Amount, DateTime.Now);

        if (_repository.OrderFulfilled(idOrder) >= 1)
            throw new Exception("Order was already fulfilled");
        
        
        return _repository.AddProductToWarehouse(orderRequest);
    }
    
    
}