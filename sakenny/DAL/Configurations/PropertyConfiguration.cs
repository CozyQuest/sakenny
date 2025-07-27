using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sakenny.DAL.Models;

namespace sakenny.Models
{
    class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(a => a.Longitude)
                   .HasColumnType("decimal(18,10)");

            builder.Property(a => a.Latitude)
                   .HasColumnType("decimal(18,10)");

            builder.Property(a => a.Price)
                   .HasColumnType("decimal(18,2)");

            builder.HasQueryFilter(a => !a.IsDeleted);

            builder.Property(p => p.Country).IsRequired();
            builder.Property(p => p.City).IsRequired();
            builder.Property(p => p.District).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.MainImage)
                   .WithOne()
                   .HasForeignKey<Property>(p => p.MainImageId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Services)
                   .WithMany(s => s.Properties)
                   .UsingEntity(j => j.ToTable("PropertyServices"));

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Property)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Rentings)
                .WithOne(r => r.Property)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
