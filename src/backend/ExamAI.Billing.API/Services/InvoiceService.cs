using Stripe;

namespace ExamAI.Billing.API.Services
{
    public interface IInvoiceService
    {
        Task CreateInvoiceAsync(string userId, int pages, decimal amount);
        Task UpdateInvoiceStatusAsync(string paymentIntentId, string status, DateTime? paidAt = null);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly IStripeClientService _stripeService;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IStripeClientService stripeService, ILogger<InvoiceService> logger)
        {
            _stripeService = stripeService;
            _logger = logger;
        }

        public async Task CreateInvoiceAsync(string userId, int pages, decimal amount)
        {
            _logger.LogInformation("Creating invoice for user {UserId}, pages: {Pages}, amount: {Amount}", userId, pages, amount);

            // 1. שמירה ראשונית ב-DB שלכם (billing.invoices) בסטטוס "open"
            // await _db.InsertInvoiceAsync(userId, pages, amount, "open");

            // 2. שליפת ה-Stripe Customer ID וה-Default Payment Method מה-DB שלכם
            string customerId = "cus_mock123456"; 
            string defaultPaymentMethodId = "pm_mock123456";

            // 3. יצירת PaymentIntent ב-Stripe ואישורו אוטומטית עם הכרטיס הקיים
            long amountILS = (long)(amount); // המרה לאגורות מתבצעת בתוך StripeClientService
            var paymentIntent = await _stripeService.ChargeCustomerAsync(customerId, amountILS, pages, $"ExamAI Invoice for {pages} pages");

            // 4. עדכון ה-Stripe PaymentIntent ID בחשבונית ב-DB
            // await _db.UpdateInvoiceStripeIdAsync(invoiceId, paymentIntent.Id);
        }

        public async Task UpdateInvoiceStatusAsync(string paymentIntentId, string status, DateTime? paidAt = null)
        {
            _logger.LogInformation("Updating invoice status for Stripe ID {Id} to {Status}", paymentIntentId, status);
            // כאן תתבצע קריאה ל-DB לעדכון הסטטוס:
            // await _db.UpdateStatusByStripeIdAsync(paymentIntentId, status, paidAt);
        }
    }
}