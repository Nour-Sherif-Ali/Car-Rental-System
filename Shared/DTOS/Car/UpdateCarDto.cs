using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared.DTOS.Car
{
    public class UpdateCarDto
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public decimal? PricePerDay { get; set; }
        public string? Imageurl { get; set; }
        public bool? Available { get; set; }
    }
}
