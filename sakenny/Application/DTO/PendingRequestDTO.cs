namespace sakenny.Application.DTO
{
    public class PendingRequestDTO
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Img { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Date { get; set; } = null!;
        public decimal Price { get; set; }
        public string Type { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
    }
}
