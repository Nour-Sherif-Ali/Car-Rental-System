using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.Booking
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarModel { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }


}
