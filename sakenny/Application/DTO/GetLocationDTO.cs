namespace sakenny.Application.DTO
{
    public class GetLocationDTO
    {
        public string Country { get; set; }
        public List<GetCityDTO> Cities { get; set; }
    }
}
