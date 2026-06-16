using System;
using System.Diagnostics.Metrics;

namespace ExamAI.Shared.Telemetry
{
    public class ExamMetricsCollector
    {
        private static readonly Meter ExamMeter = new("ExamAI.Metrics", "1.0.0");

        // 1. Counter: סופר סך הכל (רק עולה)
        private readonly Counter<long> _pagesProcessedCounter;
        
        // 2. Histogram: מודד התפלגות וזמנים (מצוין לביצועים)
        private readonly Histogram<double> _gradingDurationHistogram;
        
        // 3. Gauge: מראה מצב נוכחי (עולה ויורד, כמו מד דלק)
        private int _activeGradingJobsCount = 0;

        public ExamMetricsCollector()
        {
            _pagesProcessedCounter = ExamMeter.CreateCounter<long>(
                "examai_ocr_pages_processed_total", 
                description: "Total number of OCR pages processed");

            _gradingDurationHistogram = ExamMeter.CreateHistogram<double>(
                "examai_grading_duration_seconds", 
                unit: "s", 
                description: "Duration of exam grading jobs in seconds");

            // יצירת ObservableGauge שקורא פונקציה שמחזירה את הערך הנוכחי בכל פעם שפרומתאוס דוגם אותו
            ExamMeter.CreateObservableGauge(
                "examai_active_grading_jobs", 
                () => _activeGradingJobsCount,
                description: "Current number of active grading jobs");
        }

        // פונקציות עזר שה-Workers יוכלו לקרוא להן:
        
        public void IncrementProcessedPages(long count = 1) => _pagesProcessedCounter.Add(count);

        public void RecordGradingDuration(double seconds) => _gradingDurationHistogram.Record(seconds);

        public void IncrementActiveGradingJobs() => Interlocked.Increment(ref _activeGradingJobsCount);
        
        public void DecrementActiveGradingJobs() => Interlocked.Decrement(ref _activeGradingJobsCount);
    }
}