namespace sakenny.Application.DTO
{
    public class ReviewDTO
    {
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public int Rate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserFullName { get; set; }
        public string UserProfilePicUrl { get; set; }
    }
}
