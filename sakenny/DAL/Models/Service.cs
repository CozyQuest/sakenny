using sakenny.DAL.Interfaces;

namespace sakenny.DAL.Models
{
    public class Service : ISoftDeletable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Icon { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Property> Properties { get; set; } = new HashSet<Property>();

        // Add this for PropertySnapshot relationship
        public virtual ICollection<PropertySnapshot> PropertySnapshots { get; set; } = new HashSet<PropertySnapshot>();
    }
}
