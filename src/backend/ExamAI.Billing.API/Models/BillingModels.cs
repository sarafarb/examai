namespace ExamAI.Billing.API.Models
{
    public class UsageResponse
    {
        public string PlanName { get; set; } = "free";
        public int PagesUsedTotal { get; set; }
        public int PagesLimitFree { get; set; }
        public int PagesRemaining { get; set; }
        public bool IsUnlimited { get; set; }
        public int CurrentMonthUsage { get; set; }
        public object[] BillingHistory { get; set; } = Array.Empty<object>();
    }

    public class RecordUsageRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string ExamId { get; set; } = string.Empty;
        public string StudentExamId { get; set; } = string.Empty;
        public int Pages { get; set; }
    }

    public class PaymentMethodDto
    {
        public string Id { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Last4 { get; set; } = string.Empty;
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
        public bool IsDefault { get; set; }
    }

    public class AddPaymentMethodRequest
    {
        public string PaymentMethodId { get; set; } = string.Empty;
    }
    public class InvoicesResponse
{
    public List<InvoiceDto> Invoices { get; set; } = new();
    public int Total { get; set; }
}

    public class InvoiceDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int PagesCount { get; set; }
        public string Status { get; set; } = "open";
        public string PdfUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}