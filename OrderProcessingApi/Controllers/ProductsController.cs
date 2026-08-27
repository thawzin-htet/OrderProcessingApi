using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Data;
using OrderProcessingApi.Dtos;
using OrderProcessingApi.Models;

namespace OrderProcessingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _db.Products.Select(p => new ProductResponseDto(p.Id, p.Name, p.Sku, p.StockQuantity, p.UnitPrice)).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductResponseDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Sku = dto.Sku,
                StockQuantity = dto.StockQuantity,
                UnitPrice = dto.UnitPrice
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = product.Id }, new ProductResponseDto(product.Id, product.Name, product.Sku, product.StockQuantity, product.UnitPrice));
        }
    }
}
