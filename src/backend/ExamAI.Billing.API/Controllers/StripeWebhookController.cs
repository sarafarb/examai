using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using ExamAI.Billing.API.Configuration;
using ExamAI.Billing.API.Services;

namespace ExamAI.Billing.API.Controllers
{
    [ApiController]
    [Route("api/v1/webhooks/stripe")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly StripeSettings _settings;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(IOptions<StripeSettings> settings, IInvoiceService invoiceService, ILogger<StripeWebhookController> logger)
        {
            _settings = settings.Value;
            _invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            _logger.LogInformation("Stripe webhook received.");

            try
            {
                // אימות ה-Signature של ה-Webhook כדי לוודא שזה באמת הגיע מ-Stripe
                var stripeSignature = Request.Headers["Stripe-Signature"];
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _settings.WebhookSecret);

                _logger.LogInformation("Webhook event verified successfully: {EventType}", stripeEvent.Type);

                // טיפול בסוגי האירועים השונים
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent != null)
                        {
                            await _invoiceService.UpdateInvoiceStatusAsync(paymentIntent.Id, "paid", DateTime.UtcNow);
                            // כאן מפרסמים אירוע (Publish Event) ל-Notification Worker שישלח מייל עם הקבלה
                            _logger.LogInformation("Invoice paid event published for {Id}", paymentIntent.Id);
                        }
                        break;

                    case Events.PaymentIntentPaymentFailed:
                        var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (failedIntent != null)
                        {
                            await _invoiceService.UpdateInvoiceStatusAsync(failedIntent.Id, "open");
                            // כאן מפרסמים אירוע (Publish Event) שהתשלום נכשל כדי לשלוח התראה למשתמש
                            _logger.LogWarning("Payment failed for PaymentIntent {Id}", failedIntent.Id);
                        }
                        break;

                    case Events.CustomerUpdated:
                        var customer = stripeEvent.Data.Object as Customer;
                        if (customer != null)
                        {
                            // לוגיקת סנכרון נתוני הלקוח מול ה-DB שלכם במידה והשתנו בתוך Stripe
                            _logger.LogInformation("Syncing data for customer {CustomerId}", customer.Id);
                        }
                        break;

                    default:
                        _logger.LogInformation("Unhandled event type: {Type}", stripeEvent.Type);
                        break;
                }

                // Stripe דורשת תמיד להחזיר 200 OK, אחרת היא תמשיך לנסות לשלוח שוב ושוב
                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe Webhook signature validation failed.");
                return BadRequest(new { Error = "Invalid webhook signature" });
            }
        }
    }
}