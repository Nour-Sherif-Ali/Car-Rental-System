using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.Car
{
    public class CarResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal PricePerDay { get; set; }
        public bool Available { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; }
    }

}
