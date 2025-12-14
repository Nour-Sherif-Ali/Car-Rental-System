using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DTOS.Car;
using Shared.DTOS.Pagination;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        // GET: api/cars?brand=Tesla&minPrice=100&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CarQueryParameters parameters)
        {
            var result = await _carService.GetAllCarsAsync(parameters);
            return Ok(result);
        }

        // GET: api/cars/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(id);
                return Ok(car);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/cars
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCarDto dto)
        {
            var car = await _carService.CreateCarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        // PUT: api/cars/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCarDto dto)
        {
            try
            {
                var car = await _carService.UpdateCarAsync(id, dto);
                return Ok(car);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/cars/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _carService.DeleteCarAsync(id);
                return Ok(new { message = "Car deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/cars/5/upload-image
        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadCarImage(int id, IFormFile image)
        {
            if (image == null)
                return BadRequest("No image uploaded");

            try
            {
                await _carService.UpdateCarImageAsync(id, image);
                return Ok(new { message = "Image uploaded successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }



}
