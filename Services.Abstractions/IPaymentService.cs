using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IPaymentService
    {
       
        /// Creates a Stripe PaymentIntent for a specific booking and returns the Client Secret.
       
        Task<string> CreatePaymentIntentAsync(int bookingId, ClaimsPrincipal user);

        
        /// Handles Stripe Webhook events (payment succeeded, failed, etc.)
        
        Task HandleStripeWebhookAsync(string json, string signature);


        /// Gets payment status for a booking.
        
        Task<string> GetPaymentStatusAsync(int bookingId, ClaimsPrincipal user);

        Task ConfirmPaymentAsync(int bookingId);
        
    }

}
