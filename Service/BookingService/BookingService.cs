using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using Services.Abstractions;
using Shared.DTOS.Booking;

namespace Services.BookingService
{
    public class BookingService : IBookingService
    {
        private readonly CarRentalDbContext _db;

        public BookingService(CarRentalDbContext db)
        {
            _db = db;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            // Prevent Admin from booking
            if (user.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Admin cannot book a car");

            var car = await _db.Cars.FindAsync(dto.CarId);
            if (car == null)
                throw new Exception("Car not found");

            if (dto.EndDate < dto.StartDate)
                throw new Exception("End date must be after start date");

            // Prevent double booking (any overlapping booking not cancelled/rejected)
            bool isOverlapping = await _db.Bookings.AnyAsync(b =>
                b.CarId == dto.CarId &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Rejected &&
                dto.StartDate <= b.EndDate &&
                dto.EndDate >= b.StartDate
            );

            if (isOverlapping)
                throw new Exception("Car is already booked for this period.");

            // Prevent the same user creating duplicate pending/approved booking for same period
            bool userHasSame = await _db.Bookings.AnyAsync(b =>
                b.CarId == dto.CarId &&
                b.UserId == userId &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Rejected &&
                dto.StartDate <= b.EndDate &&
                dto.EndDate >= b.StartDate
            );

            if (userHasSame)
                throw new Exception("You already have a booking for this car in the selected period.");

            // Calculate total price
            var days = (dto.EndDate.Date - dto.StartDate.Date).Days + 1;
            var totalPrice = days * car.PricePerDay;

            var booking = new Booking
            {
                UserId = userId,
                CarId = dto.CarId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return new BookingResponseDto
            {
                Id = booking.Id,
                CarId = booking.CarId,
                CarModel = car.Name,
                TotalPrice = booking.TotalPrice,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = booking.Status
            };
        }

        public async Task<BookingResponseDto> ApproveBookingAsync(int id)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var booking = await _db.Bookings.Include(b => b.Car).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.Status != BookingStatus.Pending)
                throw new Exception("Only pending bookings can be approved.");

            // Check overlapping with other approved bookings
            bool isOverlapping = await _db.Bookings.AnyAsync(b =>
                b.Id != booking.Id &&
                b.CarId == booking.CarId &&
                b.Status == BookingStatus.Approved &&
                booking.StartDate <= b.EndDate &&
                booking.EndDate >= b.StartDate
            );

            if (isOverlapping)
                throw new Exception("Cannot approve: car already booked in this period.");

            // Approve
            booking.Status = BookingStatus.Approved;
            booking.Car.Available = false;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return new BookingResponseDto
            {
                Id = booking.Id,
                CarId = booking.CarId,
                CarModel = booking.Car.Name,
                TotalPrice = booking.TotalPrice,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = booking.Status
            };
        }

        public async Task<BookingResponseDto> RejectBookingAsync(int id)
        {
            var booking = await _db.Bookings.Include(b => b.Car).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.Status != BookingStatus.Pending)
                throw new Exception("Only pending bookings can be rejected.");

            booking.Status = BookingStatus.Rejected;
            await _db.SaveChangesAsync();

            return new BookingResponseDto
            {
                Id = booking.Id,
                CarId = booking.CarId,
                CarModel = booking.Car?.Name,
                TotalPrice = booking.TotalPrice,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = booking.Status
            };
        }

        public async Task<List<BookingResponseDto>> GetMyBookingsAsync(ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            return await _db.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Car)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    CarId = b.CarId,
                    CarModel = b.Car.Name,
                    TotalPrice = b.TotalPrice,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = b.Status
                }).ToListAsync();
        }

        public async Task<List<BookingResponseDto>> GetAllBookingsAsync()
        {
            return await _db.Bookings
                .Include(b => b.Car)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    CarId = b.CarId,
                    CarModel = b.Car.Name,
                    TotalPrice = b.TotalPrice,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = b.Status
                }).ToListAsync();
        }

        public async Task<BookingResponseDto> CancelBookingAsync(int id, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = user.IsInRole("Admin");

            var booking = await _db.Bookings.Include(b => b.Car).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                throw new Exception("Booking not found");

            if (!isAdmin && booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to cancel this booking");

            if (booking.Status == BookingStatus.Paid)
                throw new Exception("Cannot cancel a paid booking.");

            if (booking.Status == BookingStatus.Approved && booking.Car != null)
                booking.Car.Available = true;

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();


            return new BookingResponseDto
            {
                Id = booking.Id,
                CarId = booking.CarId,
                CarModel = booking.Car?.Name,
                TotalPrice = booking.TotalPrice,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = booking.Status
            };
        }
    }



}
