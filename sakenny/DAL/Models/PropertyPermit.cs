namespace sakenny.DAL.Models
{
    public class PropertyPermit
    {
        public int id { get; set; }
        public string ?AdminID { get; set; }
        public int PropertyID { get; set; }
        // "Pending" - "Accepted" - "Rejected"
        public PropertyStatus status { get; set; } = PropertyStatus.Pending;

        //navigation property many between property permit and admin
        public Admin Admin { get; set; }

        //navigation property many between property permit and propertyID
        public Property Property { get; set; }

        // ✅ Keep this - PropertyPermit is the principal side
        public virtual PropertySnapshot PropertySnapshot { get; set; }
    }
}
