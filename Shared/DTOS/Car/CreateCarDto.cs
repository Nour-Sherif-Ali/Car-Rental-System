using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared.DTOS.Car
{
    public class CreateCarDto
    {
        [Required]
        public string Name { get; set; }
        [Required] 
        public string Brand { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerDay { get; set; }
        public string? ImageUrl { get; set; }

    }
}
