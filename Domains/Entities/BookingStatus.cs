using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public static class BookingStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
        public const string Paid = "Paid";
    }

}
