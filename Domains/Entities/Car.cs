using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Car : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;    
        public decimal PricePerDay { get; set; }
        public bool Available { get; set; } = true;
        public string? ImageUrl { get; set; }

        //Navigation property
        public ICollection<Booking> Bookings { get; set; }
    }
}
