namespace OrderProcessingApi.Dtos
{
    public record ProductResponseDto(int Id, string Name, string Sku, int StockQuantity, decimal UnitPrice);
}
