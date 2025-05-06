using apbd_tutorial9.Model;

namespace apbd_tutorial9.Service;

public interface IProductWarehouseService
{
    Task<ResultIdService> AddProductToWarehouse(ProductWarehouseDTO productWarehouse);
    Task<ResultIdService> AddProductToWarehouseByProc(ProductWarehouseDTO productWarehouse);

}