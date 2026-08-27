using OrderProcessingApi.Dtos;
using OrderProcessingApi.Models;

namespace OrderProcessingApi.Services
{
    public interface IOrderService
    {
        Task<(bool Success, string? Error, Order? Order)> CreateOrderAsync(CreateOrderDto dto);
        Task<Order?> GetOrderAsync(int id);
    }
}
