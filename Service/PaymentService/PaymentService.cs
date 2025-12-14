using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistance.Data;
using Services.Abstractions;
using Stripe;

namespace Services.PaymentService
{
   

    public class PaymentService : IPaymentService
    {
        private readonly CarRentalDbContext _db;
        private readonly IConfiguration _config;

        public PaymentService(CarRentalDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        // ------------------------------------------------------------
        // 1) CREATE PAYMENT INTENT
        // ------------------------------------------------------------
        public async Task<string> CreatePaymentIntentAsync(int bookingId, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var booking = await _db.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this booking");

            if (booking.Status != BookingStatus.Approved)
                throw new Exception("Only approved bookings can be paid");

            if (booking.Status == BookingStatus.Paid)
                throw new Exception("Booking already paid");

            var amountInCents = (int)(booking.TotalPrice * 100);

            var service = new PaymentIntentService();

            var paymentIntent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                Metadata = new Dictionary<string, string>
            {
                { "BookingId", booking.Id.ToString() },
                { "UserId", userId.ToString() }
            }
            });

            booking.PaymentIntentId = paymentIntent.Id;
            await _db.SaveChangesAsync();

            return paymentIntent.ClientSecret!;
        }

        
        public async Task HandleStripeWebhookAsync(string json, string signature)
        {
            var endpointSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    endpointSecret
                );
            }
            catch
            {
                throw new Exception("Invalid Stripe webhook signature");
            }

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                await MarkBookingAsPaid(paymentIntent);
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                await HandleFailedPayment(paymentIntent);
            }
        }

        private async Task MarkBookingAsPaid(PaymentIntent paymentIntent)
        {
            var bookingId = int.Parse(paymentIntent.Metadata["BookingId"]);

            var booking = await _db.Bookings.Include(b => b.Car)
                                            .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return;

            booking.Status = BookingStatus.Paid;

            // Car remains unavailable after payment
            if (booking.Car != null)
                booking.Car.Available = false;

            await _db.SaveChangesAsync();
        }

        private async Task HandleFailedPayment(PaymentIntent paymentIntent)
        {
            var bookingId = int.Parse(paymentIntent.Metadata["BookingId"]);

            var booking = await _db.Bookings.FindAsync(bookingId);
            if (booking == null)
                return;

            booking.Status = BookingStatus.Rejected;

            await _db.SaveChangesAsync();
        }

        // ------------------------------------------------------------
        // 3) CHECK PAYMENT STATUS
        // ------------------------------------------------------------
        public async Task<string> GetPaymentStatusAsync(int bookingId, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var booking = await _db.Bookings.FindAsync(bookingId);
            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to view this payment");

            return booking.Status;
        }

        public async Task ConfirmPaymentAsync(int bookingId)
        {
            var booking = await _db.Bookings.Include(b => b.Car)
                                            .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.Status != BookingStatus.Approved)
                throw new Exception("Only approved bookings can be confirmed for payment");

            if (string.IsNullOrEmpty(booking.PaymentIntentId))
                throw new Exception("PaymentIntentId not found for this booking");

            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(booking.PaymentIntentId);

            if (paymentIntent.Status == "succeeded")
            {
                booking.Status = BookingStatus.Paid;
                if (booking.Car != null)
                    booking.Car.Available = false;

                await _db.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Payment confirmation failed. Status: {paymentIntent.Status}");
            }
        }
    }


}
