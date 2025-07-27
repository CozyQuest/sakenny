namespace sakenny.DAL.Models
{
    public class Image : ISoftDeletable
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
