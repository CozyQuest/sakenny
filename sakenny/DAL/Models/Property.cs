using sakenny.DAL.Interfaces;
namespace sakenny.DAL.Models
{
    public class Property : ISoftDeletable
    {
        public int Id { get; set; }
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
        public PropertyStatus Status { get; set; } = PropertyStatus.Pending;
        public bool IsDeleted { get; set; } = false;
        public string UserId { get; set; }
        public string MainImageUrl { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Renting>? Rentings { get; set; }
        public virtual PropertyType PropertyType { get; set; }
        public virtual HashSet<PropertyPermit> PropertyPermits { get; set; } = new HashSet<PropertyPermit>();
        public virtual HashSet<PropertySnapshot> PropertySnapshots { get; set; } = new HashSet<PropertySnapshot>();
    }
}
