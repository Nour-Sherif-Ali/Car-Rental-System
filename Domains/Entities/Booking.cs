using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string? PaymentIntentId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
