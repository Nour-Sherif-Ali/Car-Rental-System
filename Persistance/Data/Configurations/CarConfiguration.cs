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
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.Property(c => c.Name)
          .IsRequired()
          .HasMaxLength(100);

            builder.Property(c => c.Brand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.PricePerDay)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.ImageUrl)
                .HasMaxLength(300);

            builder.Property(c => c.Available)
                .HasDefaultValue(true);

            // Relation with Bookings
            builder.HasMany(c => c.Bookings)
                .WithOne(b => b.Car)
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
