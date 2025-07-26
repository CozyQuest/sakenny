using Microsoft.AspNetCore.Identity;

namespace sakenny.DAL.Models
{
    public class Admin : IdentityUser
    {
        public string UrlProfileImage { get; set; }
        public virtual HashSet<PropertyPermit>? PropertyPermits { get; set; } = new HashSet<PropertyPermit>();
    }
}