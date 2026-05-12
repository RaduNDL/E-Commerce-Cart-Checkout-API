using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models;

public record RegisterDto(
    [Required, StringLength(100)] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record CheckoutDto(
    [Required] string ShippingAddress,
    [Required] List<CheckoutItemDto> Items
);


public record CheckoutItemDto(int ProductId, int Quantity);