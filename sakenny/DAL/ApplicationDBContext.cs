using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sakenny.Models;
using System;

namespace sakenny.DAL
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) :
        base(options)
        { }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Renting> Rentings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        //public DbSet<Admin> Admins { get; set; }
        //public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUser>()
            .HasDiscriminator<string>("UserType")
            .HasValue<IdentityUser>("IdentityUser")
            .HasValue<Admin>("Admin")
            .HasValue<User>("User");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDBContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
