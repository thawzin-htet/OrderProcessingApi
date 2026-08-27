namespace OrderProcessingApi.Dtos
{
    public record CreateOrderDto(List<OrderItemDto> Items);
}
