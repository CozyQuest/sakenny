namespace sakenny.DAL.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Property> Properties { get; set; }
    }
}
