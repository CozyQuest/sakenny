namespace sakenny.Application.DTO
{
    public class EarningsItemDTO
    {
        public string PropertyTitle { get; set; }
        public string PropertyImageUrl { get; set; }
        public string RenterFullName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime TransactionDate { get; set; }
        public double? Rating { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
