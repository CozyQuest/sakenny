using sakenny.DAL.Interfaces;
namespace sakenny.DAL.Models
{
    public class PropertySnapshot : ISoftDeletable
    {
        public int Id { get; set; }
        public int? PropertyId { get; set; } // ✅ Make nullable
        
        // ✅ Add foreign key for PropertyPermit (dependent side)
        public int PropertyPermitId { get; set; }
        
        // Add timestamp for when snapshot was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // All property fields at time of snapshot
        public string Title { get; set; }
        public string Description { get; set; }
        public int PropertyTypeId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int BuildingNo { get; set; }
        public int? Level { get; set; }
        public int? FlatNo { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public double Space { get; set; }
        public decimal Price { get; set; }
        public int PeopleCapacity { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string UserId { get; set; }
        public string MainImageUrl { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Service> Services { get; set; } = new HashSet<Service>();
        public virtual PropertyType PropertyType { get; set; }
        public virtual PropertyPermit PropertyPermit { get; set; } // ✅ 1-1 relationship
        public virtual Property Property { get; set; }
    }
}
