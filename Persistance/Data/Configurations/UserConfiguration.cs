using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(150);

            builder.HasIndex(u => u.Email)
               .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();
            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);

            //RelationShip With booking 
            builder.HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
