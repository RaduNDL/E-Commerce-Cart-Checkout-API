using Microsoft.Data.SqlClient;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories;

public class UserRepository(IConfiguration config) : IUserRepository
{
    private readonly string _conn = config.GetConnectionString("DefaultConnection")!;

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("SELECT * FROM Users WHERE Email=@Email", con);
        cmd.Parameters.AddWithValue("@Email", email);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;
        return new User
        {
            Id = (int)reader["Id"],
            Name = (string)reader["Name"],
            Email = (string)reader["Email"],
            Password = (string)reader["Password"]
        };
    }

    public async Task<int> CreateAsync(User user)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "INSERT INTO Users (Name, Email, Password) OUTPUT INSERTED.Id VALUES (@Name, @Email, @Password)", con);
        cmd.Parameters.AddWithValue("@Name", user.Name);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.Parameters.AddWithValue("@Password", user.Password);
        await con.OpenAsync();
        return (int)(await cmd.ExecuteScalarAsync())!;
    }
}