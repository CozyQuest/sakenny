namespace sakenny.Application.DTO
{
    public class PendingRequestDTO
    {
        public int Id { get; set; }
        public string Img { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Date { get; set; } = null!;
        public decimal Price { get; set; }
        public string Type { get; set; } = null!;
        public string Status { get; set; } = "Pending";
    }
}
