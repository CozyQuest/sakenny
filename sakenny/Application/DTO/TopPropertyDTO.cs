namespace sakenny.Application.DTO
{
    public class TopPropertyDTO
    {
        public int PropertyId { get; set; }
        public string Img { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public double TotalRating { get; set; }
        public int CountRatings { get; set; }
    }
}
