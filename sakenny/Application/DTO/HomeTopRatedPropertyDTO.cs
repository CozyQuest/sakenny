namespace sakenny.Application.DTO
{
    public class HomeTopRatedPropertyDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal RentPerDay { get; set; }
        public double Rating { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsFavorite { get; set; } = false; // Default
        public double Area { get; set; }
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public string Location { get; set; }
    }
}
