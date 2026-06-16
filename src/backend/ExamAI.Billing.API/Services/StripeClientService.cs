using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ExamAI.Billing.API.Configuration;
using Stripe;

namespace ExamAI.Billing.API.Services
{
    public interface IStripeClientService
    {
        Task<string> CreateCustomerAsync(string userId, string email, string name);
        Task AttachPaymentMethodAsync(string customer, string paymentMethodId);
        Task SetDefaultPaymentMethodAsync(string customer, string paymentMethodId);
        Task<PaymentIntent> ChargeCustomerAsync(string customer, long amountILS, int pages, string invoiceDescription);
        Task<PaymentMethod> RetrievePaymentMethodAsync(string paymentMethodId);
        Task<StripeList<PaymentMethod>> ListPaymentMethodsAsync(string customerId);
        Task DetachPaymentMethodAsync(string paymentMethodId);
    }

    public class StripeClientService : IStripeClientService
    {
        public async Task<StripeList<PaymentMethod>> ListPaymentMethodsAsync(string customerId)
{
    var options = new PaymentMethodListOptions
    {
        Customer = customerId,
        Type = "card",
    };
    var service = new PaymentMethodService();
    return await service.ListAsync(options);
}

public async Task DetachPaymentMethodAsync(string paymentMethodId)
{
    var service = new PaymentMethodService();
    await service.DetachAsync(paymentMethodId);
}
        public StripeClientService(IOptions<StripeSettings> options)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }

        public async Task<string> CreateCustomerAsync(string userId, string email, string name)
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Metadata = new Dictionary<string, string> { { "UserId", userId } }
            };
            var service = new CustomerService();
            var customer = await service.CreateAsync(options);
            return customer.Id;
        }

        public async Task AttachPaymentMethodAsync(string customerId, string paymentMethodId)
        {
            var options = new PaymentMethodAttachOptions { Customer = customerId };
            var service = new PaymentMethodService();
            await service.AttachAsync(paymentMethodId, options);
        }

        public async Task SetDefaultPaymentMethodAsync(string customerId, string paymentMethodId)
        {
            var options = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = paymentMethodId }
            };
            var service = new CustomerService();
            await service.UpdateAsync(customerId, options);
        }

        public async Task<PaymentIntent> ChargeCustomerAsync(string customerId, long amountILS, int pages, string invoiceDescription)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amountILS * 100, // אגורות של ILS (סטרייפ מצפה למספר השלם הקטן ביותר של המטבע)
                Currency = "ils",         // דרישה מפורשת - שקלים
                Customer = customerId,
                Description = invoiceDescription,
                Metadata = new Dictionary<string, string> { { "PagesScanned", pages.ToString() } }
            };
            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public async Task<PaymentMethod> RetrievePaymentMethodAsync(string paymentMethodId)
        {
            var service = new PaymentMethodService();
            return await service.GetAsync(paymentMethodId);
        }
    }
}