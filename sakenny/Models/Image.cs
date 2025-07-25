namespace sakenny.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
