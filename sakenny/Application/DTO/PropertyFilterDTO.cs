namespace sakenny.Application.DTO
{
    public class PropertyFilterDTO
    {
        public List<int>? PropertyTypeIds { get; set; }
        public List<int>? ServiceIds { get; set; }

        public string? Country { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }

        public int? MinPeople { get; set; }
        public double? MinSpace { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? OrderBy { get; set; } // "price_asc", "price_desc", etc.
    }






}
