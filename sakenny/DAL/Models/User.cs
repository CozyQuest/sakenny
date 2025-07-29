using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.ObjectModel;

namespace sakenny.DAL.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UrlProfilePicture { get; set; }
        public string UrlIdFront { get; set; }
        public string UrlIdBack { get; set; }

        public virtual ICollection<Property>? Properties { get; set; } = new Collection<Property>();
        public virtual ICollection<Review>? Reviews { get; set; } = new Collection<Review>();
        public virtual ICollection<Renting>? Rentings { get; set; } = new Collection<Renting>();
    }
}
