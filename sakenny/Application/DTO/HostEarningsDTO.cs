namespace sakenny.Application.DTO
{
    public class HostEarningsDTO
    {
        public decimal TotalProfit { get; set; }
        public List<EarningsItemDTO> Items { get; set; }
    }
}
