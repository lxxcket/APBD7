using APBD7.Service;
using Microsoft.AspNetCore.Mvc;

namespace APBD7.Controller;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderRequest orderRequest)
    {
            int productWarehouseId = _warehouseService.AddProduct(orderRequest);
            return Ok(productWarehouseId);
        
    }
}
