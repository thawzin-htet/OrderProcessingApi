using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Models;

namespace OrderProcessingApi.Data
{
    public static class SeedData
    {
        public static async Task EnsureSeedAsync(AppDbContext db)
        {
            await db.Database.EnsureCreatedAsync();
            if (await db.Products.AnyAsync()) return;

            var products = new List<Product>
            {
                new Product { Name = "Wireless Mouse", Sku = "WM-100", StockQuantity = 50, UnitPrice = 15000m },
                new Product { Name = "Mechanical Keyboard", Sku = "MK-200", StockQuantity = 20, UnitPrice = 45000m },
                new Product { Name = "USB-C Cable", Sku = "UC-300", StockQuantity = 200, UnitPrice = 20000m },
                new Product { Name = "27\" Monitor", Sku = "MN-400", StockQuantity = 10, UnitPrice = 500000m },
                new Product { Name = "Laptop Stand", Sku = "LS-500", StockQuantity = 5, UnitPrice = 40000m },
            };

            db.Products.AddRange(products);
            await db.SaveChangesAsync();
        }
    }
}
