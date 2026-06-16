using System;
using System.Threading.Tasks;

namespace ExamAI.Billing.API.Services
{
    public class UsageSummary
    {
        public int TotalPagesScanned { get; set; }
        public int AllowedLimit { get; set; }
        public bool IsPaidPlan { get; set; }
    }

    public interface IUsageService
    {
        Task<UsageSummary> GetUsageAsync(string userId, int? month = null);
        Task AddUsageAsync(string userId, string examId, string studentExamId, int pages);
        Task<bool> CanScanMorePages(string userId);
    }

    public class UsageService : IUsageService
    {
        public async Task<UsageSummary> GetUsageAsync(string userId, int? month = null)
        {
            // כאן תבוא שאילתה ל-Database בעתיד. לצורך המבנה נחזיר Mock
            return await Task.FromResult(new UsageSummary
            {
                TotalPagesScanned = 12,
                AllowedLimit = 25,
                IsPaidPlan = false
            });
        }

        public async Task AddUsageAsync(string userId, string examId, string studentExamId, int pages)
        {
            // הוספת רשומת UsageLog ל-DB
            await Task.CompletedTask;
        }

        public async Task<bool> CanScanMorePages(string userId)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var usage = await GetUsageAsync(userId, currentMonth);

            if (usage.IsPaidPlan)
            {
                return true; // למנוי בתשלום אין הגבלה או שהחיוב מתבצע פר דף
            }

            return usage.TotalPagesScanned < usage.AllowedLimit;
        }
    }
}