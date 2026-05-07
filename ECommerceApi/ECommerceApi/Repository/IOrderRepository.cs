using ECommerceApi.Models;

namespace ECommerceApi.Repositories;

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(Order order);
}