using apbd_tutorial9.Model;
using apbd_tutorial9.Repository;

namespace apbd_tutorial9.Service;

public class ProductWarehouseService : IProductWarehouseService
{
    private readonly IProductWarehouseRepository _productWarehouseRepository;

    public ProductWarehouseService(IProductWarehouseRepository productWarehouseRepository)
    {
        _productWarehouseRepository = productWarehouseRepository;
    }
    public async Task<ResultIdService> AddProductToWarehouse(ProductWarehouseDTO productWarehouse)
    {
        var productPrice =await  _productWarehouseRepository.IsProductExists(productWarehouse.IdProduct);
        if (productPrice == null)
        {
            return ResultIdService.Fail("Product not found");
        }
        var isWarehouse = await _productWarehouseRepository.IsWarehouseExists(productWarehouse.IdWarehouse);
        if (!isWarehouse)
        {
            return ResultIdService.Fail("Warehouse not found");
        }
        var orders=await _productWarehouseRepository.GetOrders(productWarehouse.IdProduct);
        if (!orders.Any())
        {
            return ResultIdService.Fail("No orders found");
        }
        var order=orders.FirstOrDefault(o=>o.IdProduct==productWarehouse.IdProduct 
                                    && o.Amount==productWarehouse.Amount);
        if (order == null)
        {
            return ResultIdService.Fail("No order found");
        }
        var isOrderCompleted = await _productWarehouseRepository.IsOrderExists(order.IdOrder);
        if (!isOrderCompleted)
        {
            return ResultIdService.Fail("Order not found");
        }
        
        var newId=await _productWarehouseRepository.AddProductToWarehouse(productWarehouse,order.IdOrder,productPrice);
        if (newId == 0)
        {
            return ResultIdService.Fail("Failed to add product to warehouse");
        }

        return ResultIdService.Ok(newId);
    }
}