using System.ComponentModel.DataAnnotations;

namespace PriceDifferenceService.Models
{
    public class PriceDifference
    {
        [Key]
        public int Id { get; set; }
        public decimal Difference { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
