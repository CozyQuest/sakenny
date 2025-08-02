namespace sakenny.Application.DTO
{
    public class RevenueChartDTO
    {
        public IEnumerable<string> Labels { get; set; } = null!;
        public IEnumerable<object> Datasets { get; set; } = null!;
    }
}
