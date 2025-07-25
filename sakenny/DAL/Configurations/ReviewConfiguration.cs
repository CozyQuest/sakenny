using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace sakenny.Models
{
    class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Property)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Reviews)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(r => !r.IsDeleted);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Review_Rate_Range", "[Rate] >= 1 AND [Rate] <= 5");
            });
        }
    }
}
