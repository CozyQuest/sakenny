namespace sakenny.Application.DTO
{
    public class RentedPropertyDTO
    {
        public int PropertyId { get; set; }
        public string Title { get; set; }
        public string MainImageUrl { get; set; }
        public int? Rate { get; set; } 
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
