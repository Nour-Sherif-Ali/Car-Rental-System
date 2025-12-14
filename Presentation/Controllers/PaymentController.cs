using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        
        [HttpPost("create-intent/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentIntent(int bookingId)
        {
            try
            {
                var clientSecret = await _paymentService.CreatePaymentIntentAsync(bookingId, User);
                return Ok(new { ClientSecret = clientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        
        [HttpPost("confirm/{bookingId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmPayment(int bookingId)
        {
            try
            {
                await _paymentService.ConfirmPaymentAsync(bookingId);
                return Ok(new { Message = "Payment confirmed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        

       
        [HttpGet("status/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(int bookingId)
        {
            try
            {
                var status = await _paymentService.GetPaymentStatusAsync(bookingId, User);
                return Ok(new { Status = status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook([FromBody] string json)
        {
            var signature = Request.Headers["Stripe-Signature"];

            try
            {
                await _paymentService.HandleStripeWebhookAsync(json, signature);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}
