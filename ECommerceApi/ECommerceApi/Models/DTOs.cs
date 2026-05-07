namespace ECommerceApi.Models;

public record RegisterDto(string Name, string Email, string Password);
public record LoginDto(string Email, string Password);

public record CheckoutDto(
    int UserId,
    string ShippingAddress,
    List<CheckoutItemDto> Items
);

public record CheckoutItemDto(int ProductId, int Quantity);