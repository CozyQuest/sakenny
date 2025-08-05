namespace sakenny.Application.DTO
{
    public class OwnedPropertyDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainImageUrl { get; set; }
        public int PeopleCapacity { get; set; }
        public double Space { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public decimal Price { get; set; }
        public int AverageRating { get; set; }
    }
}
