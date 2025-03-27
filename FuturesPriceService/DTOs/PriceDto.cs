namespace FuturesPriceService.DTOs
{
    public class PriceDto
    {
        public string Contract { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
