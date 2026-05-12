using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerceApi.Models;
using ECommerceApi.Repositories;

namespace ECommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(
    IOrderRepository orderRepo,
    IProductRepository productRepo) : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutDto dto)
    {
        
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (dto.Items == null || dto.Items.Count == 0)
            return BadRequest(new { message = "Cart is empty." });

        decimal total = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                return BadRequest(new { message = $"Invalid quantity for product {item.ProductId}." });

            var product = await productRepo.GetByIdAsync(item.ProductId);
            if (product == null)
                return BadRequest(new { message = $"Product {item.ProductId} not found." });

            if (product.Stock < item.Quantity)
                return BadRequest(new { message = $"Not enough stock for {product.Name}." });

            total += product.Price * item.Quantity;
            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            TotalPrice = total,
            Items = orderItems
        };

        var orderId = await orderRepo.CreateOrderAsync(order);
        return Ok(new { message = "Order placed successfully.", orderId, total });
    }
}