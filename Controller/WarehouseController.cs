using apbd_tutorial9.Model;
using Microsoft.AspNetCore.Mvc;

namespace apbd_tutorial9.Controller;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(ProductWarehouseDTO product)
    {
        return Ok();
    }
}