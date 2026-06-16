using Microsoft.AspNetCore.Mvc;
using ExamAI.Billing.API.Models;

namespace ExamAI.Billing.API.Controllers
{
    [ApiController]
    [Route("api/v1/billing/invoices")]
    public class InvoicesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInvoices([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // במציאות נשלוף מה-DB לפי מזהה המשתמש מה-Token ונעשה פגינציה (Skip ו-Take)
            string userId = "mock-user-uuid";

            // יצירת מידע דמה (Mock) לפי ה-Response שנדרש במשימה
            var mockInvoices = new List<InvoiceDto>
            {
                new InvoiceDto 
                { 
                    Id = Guid.NewGuid().ToString(), 
                    Amount = 4.20m, 
                    PagesCount = 21, 
                    Status = "paid", 
                    PdfUrl = $"https://examai-billing.s3.amazonaws.com/receipts/mock-id.pdf", 
                    CreatedAt = DateTime.UtcNow.AddDays(-2) 
                }
            };

            var response = new InvoicesResponse
            {
                Invoices = mockInvoices,
                Total = 15 // סך הכל החשבוניות שלו ב-DB
            };

            return Ok(response);
        }

        [HttpGet("{id}/download")]
        public IActionResult DownloadInvoice(string id)
        {
            // 1. כאן יוצרים חשבונית PDF (או משתמשים ב-Stripe Hosted Invoice URL)
            // 2. מעלים ל-S3 ומייצרים Presigned URL שתקף ל-15 דקות בדיוק
            
            string mockS3PresignedUrl = $"https://examai-billing.s3.amazonaws.com/receipts/{id}.pdf?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Expires=900";

            return Ok(new { DownloadUrl = mockS3PresignedUrl });
        }
    }
}