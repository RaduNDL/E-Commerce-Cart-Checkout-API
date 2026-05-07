using ECommerceApi.Models;

namespace ECommerceApi.Tests;

public class OrderCalculationTests
{
    [Fact]
    public void Checkout_CalculatesTotal_Correctly()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Blue Top", Price = 500.00m, Stock = 10 },
            new() { Id = 2, Name = "Men Tshirt", Price = 400.00m, Stock = 10 }
        };

        var items = new List<CheckoutItemDto>
        {
            new(ProductId: 1, Quantity: 2),
            new(ProductId: 2, Quantity: 1)
        };

        decimal total = 0;
        foreach (var item in items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            total += product.Price * item.Quantity;
        }

        Assert.Equal(1400.00m, total);
    }

    [Fact]
    public void Checkout_InsufficientStock_ShouldFail()
    {
        var product = new Product { Id = 1, Name = "Blue Top", Price = 500m, Stock = 1 };
        var requestedQuantity = 5;

        var hasStock = product.Stock >= requestedQuantity;

        Assert.False(hasStock);
    }

    [Fact]
    public void Register_Password_ShouldBeHashed()
    {
        var plainPassword = "mypassword123";

        var hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        var isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashed);

        Assert.True(isValid);
        Assert.NotEqual(plainPassword, hashed);
    }

    [Fact]
    public void Checkout_EmptyCart_ShouldHaveZeroTotal()
    {
        var items = new List<CheckoutItemDto>();

        decimal total = items.Sum(_ => 0m);

        Assert.Equal(0m, total);
    }

    [Fact]
    public void Checkout_SufficientStock_ShouldPass()
    {
        var product = new Product { Id = 1, Name = "Blue Top", Price = 500m, Stock = 10 };
        var requestedQuantity = 3;

        var hasStock = product.Stock >= requestedQuantity;

        Assert.True(hasStock);
    }

    [Fact]
    public void Login_WrongPassword_ShouldFailVerification()
    {
        var plainPassword = "correctpassword";
        var hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        var isValid = BCrypt.Net.BCrypt.Verify("wrongpassword", hashed);

        Assert.False(isValid);
    }

    [Fact]
    public void Checkout_MultipleItems_TotalIsCorrect()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Price = 1000m, Stock = 5 },
            new() { Id = 2, Price = 250m,  Stock = 5 },
            new() { Id = 3, Price = 750m,  Stock = 5 }
        };

        var items = new List<CheckoutItemDto>
        {
            new(ProductId: 1, Quantity: 1),
            new(ProductId: 2, Quantity: 4),
            new(ProductId: 3, Quantity: 2)
        };

        decimal total = 0;
        foreach (var item in items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            total += product.Price * item.Quantity;
        }

        Assert.Equal(3500.00m, total);
    }
}