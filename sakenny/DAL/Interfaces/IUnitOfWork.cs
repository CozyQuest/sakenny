using Microsoft.AspNetCore.Identity;
using sakenny.DAL.Models;

namespace sakenny.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<User> Users { get; }
        IDeleteUpdate<Property> Properties { get; }
        IDeleteUpdate<Image> Images { get; }
        IBaseRepository<Renting> Rentings { get; }
        IDeleteUpdate<Review> Reviews { get; }
        IDeleteUpdate<Service> Services { get; }
        IDeleteUpdate<PropertyType> PropertyTypes { get; }
        IBaseRepository<PropertyPermit> PropertyPermits { get; }
        IDeleteUpdate<PropertySnapshot> PropertySnapshots { get; }
        UserManager<IdentityUser> userManager { get; }
        RoleManager<IdentityRole> roleManager { get; }
        Task<int> SaveChangesAsync();
    }
}
