using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace sakenny.Models
{
    class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .IsRequired();

            builder.HasMany(s => s.Properties)
                   .WithMany(p => p.Services)
                   .UsingEntity(j => j.ToTable("PropertyServices"));

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
