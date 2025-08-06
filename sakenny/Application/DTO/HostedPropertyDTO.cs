namespace sakenny.Application.DTO
{
    public class HostedPropertyDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Area { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Location { get; set; } = string.Empty;
    }   
}