namespace PriceDifferenceService.DTOs
{
    public class PriceDto
    {
        public string Contract { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
