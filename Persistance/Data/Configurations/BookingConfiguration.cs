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
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.TotalPrice)
           .HasColumnType("decimal(18,2)")
           .IsRequired();

            builder.Property(b => b.StartDate)
                .IsRequired();

            builder.Property(b => b.EndDate)
                .IsRequired();
        }
    }
}
