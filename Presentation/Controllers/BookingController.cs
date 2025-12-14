using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DTOS.Booking;



namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

       
        [HttpPost]
        public async Task<ActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                var result = await _bookingService.CreateBookingAsync(dto, User);
                return CreatedAtAction(nameof(GetMyBookings), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

      
        [HttpGet("my-bookings")]
        public async Task<ActionResult> GetMyBookings()
        {
            try
            {
                var bookings = await _bookingService.GetMyBookingsAsync(User);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            await _bookingService.ApproveBookingAsync(id);
            return Ok(new { message = "Booking approved successfully" });
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectBooking(int id)
        {
            await _bookingService.RejectBookingAsync(id);
            return Ok(new { message = "Booking rejected" });
        }




        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelBooking(int id)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(id, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(); // لازم يطلع 403
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
