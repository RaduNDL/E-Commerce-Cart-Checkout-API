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

        using var cmdOrder = new SqlCommand(
            "INSERT INTO Orders (UserId, TotalPrice, ShippingAddress) OUTPUT INSERTED.Id VALUES (@UserId, @TotalPrice, @Address)", con);
        cmdOrder.Parameters.AddWithValue("@UserId", order.UserId);
        cmdOrder.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
        cmdOrder.Parameters.AddWithValue("@Address", order.ShippingAddress);
        int orderId = (int)(await cmdOrder.ExecuteScalarAsync())!;

        foreach (var item in order.Items)
        {
            using var cmdItem = new SqlCommand(
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (@OrderId, @ProductId, @Qty, @Price)", con);
            cmdItem.Parameters.AddWithValue("@OrderId", orderId);
            cmdItem.Parameters.AddWithValue("@ProductId", item.ProductId);
            cmdItem.Parameters.AddWithValue("@Qty", item.Quantity);
            cmdItem.Parameters.AddWithValue("@Price", item.UnitPrice);
            await cmdItem.ExecuteNonQueryAsync();
        }

        return orderId;
    }
}