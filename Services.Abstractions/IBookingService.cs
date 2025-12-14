using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOS.Booking;

namespace Services.Abstractions
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto, ClaimsPrincipal user);
        Task<BookingResponseDto> ApproveBookingAsync(int id);
        Task<BookingResponseDto> RejectBookingAsync(int id);
        Task<List<BookingResponseDto>> GetMyBookingsAsync(ClaimsPrincipal user);
        
        Task<List<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto> CancelBookingAsync(int id, ClaimsPrincipal user);
    }

}
