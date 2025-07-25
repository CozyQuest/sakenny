using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace sakenny.Models
{
    public class PropertyPermitConfiguration : IEntityTypeConfiguration<PropertyPermit>
    {
        public void Configure(EntityTypeBuilder<PropertyPermit> builder)
        {
            builder.HasKey(p => p.id);

            // Configure relationship with Admin
            builder
                .HasOne(p => p.Admin)
                .WithMany(a => a.PropertyPermits) // Reference the collection in Admin
                .HasForeignKey(p => p.AdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Property
            builder
                .HasOne(p => p.Property)
                .WithMany(prop => prop.PropertyPermits) // Reference the collection in Property
                .HasForeignKey(p => p.PropertyID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.status)
                   .IsRequired();
        }
    }
}