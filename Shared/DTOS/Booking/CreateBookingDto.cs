using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.Booking
{
    public class CreateBookingDto
    {
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
