namespace sakenny.Application.DTO
{
    public class PropertyCheckoutDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public decimal Price { get; set; }
        public string MainImageURL { get; set; }
        public List<DateTime> RentedDates { get; set; } = new();
    }
}
