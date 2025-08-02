namespace sakenny.Application.DTO
{
    public class StatDTO
    {
        public string Title { get; set; } = null!;
        public decimal Value { get; set; }
        public string? Currency { get; set; }
        public string Icon { get; set; } = null!;
    }
}
