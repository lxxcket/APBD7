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
        int productWarehouseId = 0;
        try
        {
            productWarehouseId = _warehouseService.AddProduct(orderRequest);
        }
        catch (Exception e)
        {
            return BadRequest();
        }
            
            
        return Ok(productWarehouseId);
    }

    [HttpPost("callprocedure")]
    public IActionResult CallProcedure(OrderRequest orderRequest)
    {
        int id = 0;
        try
        {
            id = _warehouseService.AddProductUsingProcedure(orderRequest);
        }
        catch (Exception e)
        {
            return BadRequest();
        }

        return Ok(id);
    }
}
