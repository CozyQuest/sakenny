namespace sakenny.DAL.Models
{
    public class Renting
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
