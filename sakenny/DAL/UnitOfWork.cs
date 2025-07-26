using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;
        public IBaseRepository<Renting> Rentings { get; private set; }
        public IBaseRepository<PropertyPermit> PropertyPermits { get; private set; }
        public IDeleteUpdate<Property> Properties { get; private set; }
        public IDeleteUpdate<Image> Images { get; private set; }
        public IDeleteUpdate<Review> Reviews { get; private set; }
        public IDeleteUpdate<Service> Services { get; private set; }
        public IDeleteUpdate<PropertyType> PropertyTypes { get; private set; }

        public UnitOfWork(ApplicationDBContext context, IBaseRepository<Renting> rentings, IBaseRepository<PropertyPermit> propertyPermits, IDeleteUpdate<Property> properties, IDeleteUpdate<Image> images, IDeleteUpdate<Review> reviews, IDeleteUpdate<Service> services, IDeleteUpdate<PropertyType> propertyTypes)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Rentings = rentings;
            PropertyPermits = propertyPermits;
            Properties = properties;
            Images = images;
            Reviews = reviews;
            Services = services;
            PropertyTypes = propertyTypes;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
