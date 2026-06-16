namespace ExamAI.Exam.API.Services
{
    public interface IFileValidationService
    {
        bool ValidateMagicBytes(Stream fileStream, string contentType);
    }

    public class FileValidationService : IFileValidationService
    {
        private static readonly Dictionary<string, byte[]> MagicBytesMap = new()
        {
            { "application/pdf", new byte[] { 0x25, 0x50, 0x44, 0x46 } },                 // %PDF
            { "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },                             // JPEG
            { "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47 } },                         // PNG
            { "image/tiff", new byte[] { 0x49, 0x49, 0x2A, 0x00 } }                        // TIFF (Little Endian)
        };

        public bool ValidateMagicBytes(Stream fileStream, string contentType)
        {
            if (!MagicBytesMap.ContainsKey(contentType)) return false;

            var requiredBytes = MagicBytesMap[contentType];
            var buffer = new byte[requiredBytes.Length];

            // שמירת המיקום המקורי של הסטרים כדי לא לקלקל לקריאות הבאות
            long originalPosition = fileStream.Position;
            fileStream.Position = 0;
            
            int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Position = originalPosition; // החזרת הסטרים להתחלה

            if (bytesRead < requiredBytes.Length) return false;

            for (int i = 0; i < requiredBytes.Length; i++)
            {
                if (buffer[i] != requiredBytes[i]) return false;
            }

            return true;
        }
    }
}