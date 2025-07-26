using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace sakenny.Models
{
    class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            // Remove HasKey - it's inherited from IdentityUser
            // Remove Email and PhoneNumber - they're inherited properties

            //builder.Property(a => a.FName).IsRequired();
            //builder.Property(a => a.LName).IsRequired();
            
            // Remove the AdminSpecificProperty line as it doesn't exist in the model
        }
    }
}
