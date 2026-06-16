using System.Net.Sockets;
using System.Text;

namespace ExamAI.Exam.API.Services
{
    public class ScanResult
    {
        public bool IsSafe { get; set; }
        public string? ThreatName { get; set; }
    }

    public interface IVirusScanService
    {
        Task<ScanResult> ScanFileAsync(Stream fileStream);
    }

    public class VirusScanService : IVirusScanService
    {
        private readonly ILogger<VirusScanService> _logger;
        private const string ClamAlHost = "127.0.0.1";
        private const int ClamAvPort = 3310;

        public VirusScanService(ILogger<VirusScanService> logger)
        {
            _logger = logger;
        }

        public async Task<ScanResult> ScanFileAsync(Stream fileStream)
        {
            try
            {
                using var client = new TcpClient();
                // הגדרת Timeout של 30 שניות כפי שנדרש
                var connectTask = client.ConnectAsync(ClamAlHost, ClamAvPort);
                if (await Task.WhenAny(connectTask, Task.Delay(30000)) != connectTask)
                {
                    throw new TimeoutException("ClamAV connection timed out.");
                }

                using var stream = client.GetStream();
                long originalPosition = fileStream.Position;
                fileStream.Position = 0;

                // שליחת פקודת הזרמה (INSTREAM) ל-ClamAV לפי הפרוטוקול שלו
                byte[] command = Encoding.ASCII.GetBytes("zINSTREAM\0");
                await stream.WriteAsync(command, 0, command.Length);

                byte[] chunkBuffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(chunkBuffer, 0, chunkBuffer.Length)) > 0)
                {
                    // פרוטוקול ClamAV דורש לשלוח את אורך ה-chunk ב-4 בתים (Big Endian) לפני המידע עצמו
                    byte[] chunkSizeHeader = BitConverter.GetBytes(bytesRead);
                    if (BitConverter.IsLittleEndian) Array.Reverse(chunkSizeHeader);

                    await stream.WriteAsync(chunkSizeHeader, 0, chunkSizeHeader.Length);
                    await stream.WriteAsync(chunkBuffer, 0, bytesRead);
                }

                // סיום ההזרמה ע"י שליחת 4 בתים של אפס
                byte[] endMarker = [0, 0, 0, 0];
                await stream.WriteAsync(endMarker, 0, endMarker.Length);

                // קריאת התשובה מהסוקט
                using var reader = new StreamReader(stream, Encoding.ASCII);
                string response = await reader.ReadToEndAsync();
                
                fileStream.Position = originalPosition; // החזרת הסטרים למצבו המקורי

                if (response.Contains("FOUND"))
                {
                    string threatName = response.Replace("stream:", "").Replace("FOUND", "").Trim();
                    return new ScanResult { IsSafe = false, ThreatName = threatName };
                }

                return new ScanResult { IsSafe = true };
            }
            catch (Exception ex)
            {
                // Degraded mode: אם האנטי וירוס לא זמין, רושמים אזהרה ומאפשרים את הקובץ
                _logger.LogWarning(ex, "ClamAV Anti-Virus is unavailable. Running in degraded mode (File allowed).");
                return new ScanResult { IsSafe = true };
            }
        }
    }
}