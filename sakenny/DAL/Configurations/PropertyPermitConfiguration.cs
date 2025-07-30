using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sakenny.DAL.Models;

namespace sakenny.Models
{
    public class PropertyPermitConfiguration : IEntityTypeConfiguration<PropertyPermit>
    {
        public void Configure(EntityTypeBuilder<PropertyPermit> builder)
        {
            builder.HasKey(pp => pp.id);

            // Configure relationship with Admin
            builder
                .HasOne(p => p.Admin)
                .WithMany(a => a.PropertyPermits) // Reference the collection in Admin
                .HasForeignKey(p => p.AdminID)
                .OnDelete(DeleteBehavior.NoAction);

            // ✅ CRITICAL FIX: Change from CASCADE to RESTRICT
            builder
                .HasOne(p => p.Property)
                .WithMany(prop => prop.PropertyPermits) // Reference the collection in Property
                .HasForeignKey(p => p.PropertyID)
                .OnDelete(DeleteBehavior.NoAction); // ✅ Changed from Cascade to Restrict

            builder.Property(p => p.status)
                   .IsRequired();
            
            builder.HasIndex(pp => pp.PropertyID);
        }
    }
}