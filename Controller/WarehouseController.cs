using apbd_tutorial9.Model;
using apbd_tutorial9.Service;
using Microsoft.AspNetCore.Mvc;

namespace apbd_tutorial9.Controller;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IProductWarehouseService _productWarehouseService;

    public WarehouseController(IProductWarehouseService productWarehouseService)
    {
        _productWarehouseService = productWarehouseService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(ProductWarehouseDTO productWarehouse)
    {
        var res= await _productWarehouseService.AddProductToWarehouse(productWarehouse);
        if (!res.Success)
        {
            return BadRequest(res.Message);
        }
        return Ok(res.Id);
    }
}