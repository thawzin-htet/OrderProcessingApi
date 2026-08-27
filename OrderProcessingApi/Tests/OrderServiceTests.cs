using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Data;
using OrderProcessingApi.Dtos;
using OrderProcessingApi.Models;
using OrderProcessingApi.Services;
using Xunit;

namespace OrderProcessingApi.Tests
{
    public class OrderServiceTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly DbContextOptions<AppDbContext> _opts;

        public OrderServiceTests()
        {
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();
            _opts = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_conn).Options;

            using var db = new AppDbContext(_opts);
            db.Database.EnsureCreated();
            db.Products.AddRange(
                new Product { Name = "P1", Sku = "p1", StockQuantity = 5, UnitPrice = 10m },
                new Product { Name = "P2", Sku = "p2", StockQuantity = 2, UnitPrice = 20m }
            );
            db.SaveChanges();
        }

        [Fact]
        public async Task SuccessfulOrder_DeductsStock()
        {
            using var db = new AppDbContext(_opts);
            var svc = new OrderService(db);
            var dto = new CreateOrderDto(new List<OrderItemDto> { new OrderItemDto(1, 3) });
            var (success, error, order) = await svc.CreateOrderAsync(dto);
            Assert.True(success);
            Assert.Null(error);
            Assert.NotNull(order);
            var p = await db.Products.FindAsync(1);
            Assert.Equal(2, p!.StockQuantity);
        }

        [Fact]
        public async Task RejectsOrder_WhenQuantityExceedsStock()
        {
            using var db = new AppDbContext(_opts);
            var svc = new OrderService(db);
            var dto = new CreateOrderDto(new List<OrderItemDto> { new OrderItemDto(2, 5) });
            var (success, error, order) = await svc.CreateOrderAsync(dto);
            Assert.False(success);
            Assert.Contains("Insufficient stock", error);
            var p = await db.Products.FindAsync(2);
            Assert.Equal(2, p!.StockQuantity); // unchanged
        }

        [Fact]
        public async Task Transaction_Rollback_OnPartialFailure()
        {
            using var db = new AppDbContext(_opts);
            var svc = new OrderService(db);
            // item 1 ok, item 999 missing -> should rollback and not deduct item 1
            var dto = new CreateOrderDto(new List<OrderItemDto> { new OrderItemDto(1, 1), new OrderItemDto(999, 1) });
            var (success, error, order) = await svc.CreateOrderAsync(dto);
            Assert.False(success);
            var p = await db.Products.FindAsync(1);
            Assert.Equal(5, p!.StockQuantity); // unchanged because rolled back
        }

        public void Dispose()
        {
            _conn?.Dispose();
        }
    }
}
