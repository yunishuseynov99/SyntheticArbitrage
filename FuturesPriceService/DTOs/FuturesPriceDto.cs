namespace FuturesPriceService.DTOs
{
    public class FuturesPriceDto
    {
        public string Contract { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
