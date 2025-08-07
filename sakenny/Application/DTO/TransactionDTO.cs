namespace sakenny.Application.DTO
{
    public class TransactionDTO
    {
        public int PropertyId { get; set; }
        public string Img { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string HostId { get; set; } = null!;
        public string HostName { get; set; } = null!;
        public string GuestId { get; set; } = null!;
        public string GuestName { get; set; } = null!;
        public string Price { get; set; } = null!;
        public string Date { get; set; } = null!;
    }
}
