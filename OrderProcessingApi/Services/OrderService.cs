using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Data;
using OrderProcessingApi.Dtos;
using OrderProcessingApi.Models;

namespace OrderProcessingApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        public OrderService(AppDbContext db) => _db = db;

        public async Task<(bool Success, string? Error, Order? Order)> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any()) return (false, "No items provided", null);

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Load products and validate quantities
                var productIds = dto.Items.Select(i => i.ProductId).ToArray();
                var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

                foreach (var item in dto.Items)
                {
                    if (!products.TryGetValue(item.ProductId, out var prod))
                        return (false, $"Product {item.ProductId} not found", null);

                    if (item.Quantity <= 0) return (false, "Quantity must be > 0", null);

                    if (item.Quantity > prod.StockQuantity)
                        return (false, $"Insufficient stock for product {prod.Name}", null);
                }

                var order = new Order { CreatedAt = DateTime.UtcNow, Status = "Created" };
                decimal total = 0m;

                foreach (var item in dto.Items)
                {
                    var prod = products[item.ProductId];
                    prod.StockQuantity -= item.Quantity; // optimistic concurrency via RowVersion
                    var line = new OrderItem
                    {
                        ProductId = prod.Id,
                        Quantity = item.Quantity,
                        UnitPrice = prod.UnitPrice
                    };
                    order.Items.Add(line);
                    total += item.Quantity * prod.UnitPrice;
                }

                order.TotalAmount = total;
                _db.Orders.Add(order);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return (true, null, order);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync();
                return (false, "Concurrency conflict while processing order. Please retry.", null);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, ex.Message, null);
            }
        }

        public Task<Order?> GetOrderAsync(int id)
        {
            return _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
