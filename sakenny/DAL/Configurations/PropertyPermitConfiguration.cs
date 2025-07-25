using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sakenny.Models;

namespace sakenny.Models
{
    public class PropertyPermitConfiguration : IEntityTypeConfiguration<PropertyPermit>
    {
        public void Configure(EntityTypeBuilder<PropertyPermit> builder)
        {
            builder.HasKey(p => p.id);

            // AdminID is the foreign key for Admin
            builder
                .HasOne(p => p.Admins)
                .WithMany()
                .HasForeignKey(p => p.AdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // PropertyID is the foreign key for Property
            builder
                .HasOne(p => p.Properties)
                .WithMany() 
                .HasForeignKey(p => p.PropertyID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.status)
                   .IsRequired();
        }
    }
}
