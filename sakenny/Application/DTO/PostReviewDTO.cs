using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class PostReviewDTO
    {
        [Required(ErrorMessage = "Review text is required.")]
        public string ReviewText { get; set; }
        public int? Rate { get; set; }

    }
}
