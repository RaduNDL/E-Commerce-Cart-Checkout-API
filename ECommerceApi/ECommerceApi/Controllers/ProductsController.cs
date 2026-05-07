using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Repositories;

namespace ECommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository productRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await productRepo.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productRepo.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}