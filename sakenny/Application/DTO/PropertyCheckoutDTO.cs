namespace sakenny.Application.DTO
{
    public class PropertyCheckoutDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Space { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public decimal Price { get; set; }
        public string MainImageURL { get; set; }
        public int Rating { get; set; }
        public int RatingCount { get; set; }
        public List<DateTime> RentedDates { get; set; } = new();
    }
}
