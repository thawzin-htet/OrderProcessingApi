using Microsoft.AspNetCore.Mvc;
using OrderProcessingApi.Dtos;
using OrderProcessingApi.Services;

namespace OrderProcessingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpPost]
        public async Task<IActionResult> Post(CreateOrderDto dto)
        {
            var (success, error, order) = await _orderService.CreateOrderAsync(dto);
            if (!success) return BadRequest(new { error });
            return CreatedAtAction(nameof(Get), new { id = order!.Id }, new { order.Id, order.CreatedAt, order.TotalAmount, order.Status });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null) return NotFound();
            var items = order.Items.Select(i => new { i.ProductId, i.Quantity, i.UnitPrice, LineTotal = i.Quantity * i.UnitPrice });
            return Ok(new { order.Id, order.CreatedAt, order.TotalAmount, order.Status, Items = items });
        }
    }
}
