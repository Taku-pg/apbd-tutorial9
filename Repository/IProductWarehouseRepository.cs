using apbd_tutorial9.Model;

namespace apbd_tutorial9.Repository;

public interface IProductWarehouseRepository
{
    Task<decimal?> IsProductExists(int productId);
    Task<bool> IsWarehouseExists(int warehouseId);
    Task<List<OrderDTO>> GetOrders(int productId);
    Task<bool> IsOrderExists(int orderId);
    Task<bool> UpdateOrder(int orderId);
    Task<bool> InsertProductToWarehouse(ProductWarehouseDTO productWarehouse,int orderId,decimal price);
}