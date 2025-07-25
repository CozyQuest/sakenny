using Microsoft.AspNetCore.Identity;

namespace sakenny.Models
{
    public class Admin : IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string UrlProfileImage { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual HashSet<PropertyPermit> PropertyPermits { get; set; } = new HashSet<PropertyPermit>();
    }
}