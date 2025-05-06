using apbd_tutorial9.Model;
using apbd_tutorial9.Service;
using Microsoft.Data.SqlClient;

namespace apbd_tutorial9.Repository;

public class ProductWarehouseRepository : IProductWarehouseRepository
{
    private readonly string _connectionString;

    public ProductWarehouseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("apbd");
    }
    
    public async Task<decimal?> IsProductExists(int productId)
    {
        string sql = "SELECT price FROM Product WHERE ProductId = @ProductId";
        
        using(SqlConnection conn=new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@ProductId", productId);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            if(result is null)
                return null;
            return Convert.ToDecimal(result);
        }
    }

    public async Task<bool> IsWarehouseExists(int warehouseId)
    {
        string sql = "SELECT 1 FROM Warehouse WHERE EXISTS(SELECT * FROM Warehouse WHERE IdWarehouse = @WarehouseId)";
        
        using(SqlConnection conn=new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@WarehouseId",warehouseId);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
    }

    public async Task<List<OrderDTO>> GetOrders(int productId)
    {
        var orders = new List<OrderDTO>();
        string sql = "SELECT * FROM Order WHERE ProductId = @ProductId";
        using(SqlConnection conn=new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@ProductId",productId);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                orders.Add(new OrderDTO
                {
                    IdOrder = reader.GetInt32(reader.GetOrdinal("IdOrder")),
                    IdProduct = reader.GetInt32(reader.GetOrdinal("IdProduct")),
                    Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
        }
        
        return orders;
    }

    public async Task<bool> IsOrderExists(int orderId)
    {
        string sql = "SELECT 1 FROM Product_Warehouse WHERE EXISTS(SELECT * FROM Product_Warehouse WHERE IdOrder = @OrderId)";
        
        using(SqlConnection conn=new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@OrderId",orderId);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
    }

    public async Task<bool> UpdateOrder(int orderId)
    {
        string sql = "UPDATE Order SET FullfilledAt = @CurrentDate WHERE IdOrder = @IdOrder";
        using(SqlConnection conn=new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@IdOrder",orderId);
            cmd.Parameters.AddWithValue("@CurrentDate",DateTime.Now);
            await conn.OpenAsync();
            var res=await cmd.ExecuteNonQueryAsync();
            return res == 1;
        }
    }

    public async Task<bool> InsertProductToWarehouse(ProductWarehouseDTO productWarehouse,int orderId,decimal price)
    {
        string sql = @"INSERT INTO Product_Warehouse (IdWarehouse,IdProduct,IdOrder,Amount,Price,CreatedAt) 
                        VALUES (@IdWarehouse,@IdProduct,@IdOrder,@Amount,@Price,@CreatedAt)";
        
        using(SqlConnection conn=new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@IdWarehouse",productWarehouse.IdWarehouse);
            cmd.Parameters.AddWithValue("@IdProduct",productWarehouse.IdProduct);
            cmd.Parameters.AddWithValue("@IdOrder",orderId);
            cmd.Parameters.AddWithValue("@Amount",productWarehouse.Amount);
            cmd.Parameters.AddWithValue("@Price",price*productWarehouse.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt",DateTime.Now);
            await conn.OpenAsync();
            var res=await cmd.ExecuteNonQueryAsync();
            return res == 1;
        }
    }

    public async Task<int> AddProductToWarehouse(ProductWarehouseDTO productWarehouse,int orderId,decimal? price)
    {
        SqlConnection conn=new SqlConnection(_connectionString);
        SqlCommand cmd = new SqlCommand();
        
        cmd.Connection = conn;
        await conn.OpenAsync();
        
        var tran= await conn.BeginTransactionAsync();
        cmd.Transaction = tran as SqlTransaction;

        try
        {
            cmd.CommandText = "UPDATE Order SET FullfilledAt = @CurrentDate WHERE IdOrder = @IdOrder";
            cmd.Parameters.AddWithValue("@IdOrder",orderId); 
            cmd.Parameters.AddWithValue("@CurrentDate",DateTime.Now); 
            await cmd.ExecuteNonQueryAsync(); 
            
            cmd.Parameters.Clear();
            
            cmd.CommandText=@"INSERT INTO Product_Warehouse (IdWarehouse,IdProduct,IdOrder,Amount,Price,CreatedAt) 
                        VALUES (@IdWarehouse,@IdProduct,@IdOrder,@Amount,@Price,@CreatedAt);
                        SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@IdWarehouse",productWarehouse.IdWarehouse);
            cmd.Parameters.AddWithValue("@IdProduct",productWarehouse.IdProduct);
            cmd.Parameters.AddWithValue("@IdOrder",orderId);
            cmd.Parameters.AddWithValue("@Amount",productWarehouse.Amount);
            cmd.Parameters.AddWithValue("@Price",price*productWarehouse.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt",DateTime.Now);
            await cmd.ExecuteNonQueryAsync();
            var newId =await cmd.ExecuteScalarAsync();
            
            await tran.CommitAsync();
            
            return Convert.ToInt32(newId);
        }
        catch (Exception ex)
        {
            tran.Rollback();
            return 0;
        }
    }
}