using Microsoft.Data.SqlClient;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories;

public class ProductRepository(IConfiguration config) : IProductRepository
{
    private readonly string _conn = config.GetConnectionString("DefaultConnection")!;

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var list = new List<Product>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("SELECT * FROM Products", con);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(Map(reader));
        return list;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("SELECT * FROM Products WHERE Id=@Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    private static Product Map(SqlDataReader r) => new()
    {
        Id = (int)r["Id"],
        Name = (string)r["Name"],
        Description = r["Description"] as string,
        Price = (decimal)r["Price"],
        Category = r["Category"] as string,
        ImageUrl = r["ImageUrl"] as string,
        Stock = (int)r["Stock"]
    };
}