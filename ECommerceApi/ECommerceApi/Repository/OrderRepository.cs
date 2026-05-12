using Microsoft.Data.SqlClient;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories;

public class OrderRepository(IConfiguration config) : IOrderRepository
{
    private readonly string _conn = config.GetConnectionString("DefaultConnection")!;

    public async Task<int> CreateOrderAsync(Order order)
    {
        using var con = new SqlConnection(_conn);
        await con.OpenAsync();
        using var tx = (SqlTransaction)await con.BeginTransactionAsync();

        try
        {
            
            using var cmdOrder = new SqlCommand(
                "INSERT INTO Orders (UserId, TotalPrice, ShippingAddress) OUTPUT INSERTED.Id VALUES (@UserId, @TotalPrice, @Address)",
                con, tx);
            cmdOrder.Parameters.AddWithValue("@UserId", order.UserId);
            cmdOrder.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
            cmdOrder.Parameters.AddWithValue("@Address", order.ShippingAddress);
            int orderId = (int)(await cmdOrder.ExecuteScalarAsync())!;

            foreach (var item in order.Items)
            {
            
                using var cmdItem = new SqlCommand(
                    "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (@OrderId, @ProductId, @Qty, @Price)",
                    con, tx);
                cmdItem.Parameters.AddWithValue("@OrderId", orderId);
                cmdItem.Parameters.AddWithValue("@ProductId", item.ProductId);
                cmdItem.Parameters.AddWithValue("@Qty", item.Quantity);
                cmdItem.Parameters.AddWithValue("@Price", item.UnitPrice);
                await cmdItem.ExecuteNonQueryAsync();

                using var cmdStock = new SqlCommand(
                    "UPDATE Products SET Stock = Stock - @Qty WHERE Id = @ProductId AND Stock >= @Qty",
                    con, tx);
                cmdStock.Parameters.AddWithValue("@Qty", item.Quantity);
                cmdStock.Parameters.AddWithValue("@ProductId", item.ProductId);
                int rows = await cmdStock.ExecuteNonQueryAsync();

                if (rows == 0)
                    throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}.");
            }

            await tx.CommitAsync();
            return orderId;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}