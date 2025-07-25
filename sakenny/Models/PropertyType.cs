namespace sakenny.Models
{
    public class PropertyType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
