using Amazon.S3;
using Amazon.S3.Model;

namespace ExamAI.Exam.API.Services
{
    public interface IS3FileService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string userId, string examId, string type);
        string GetPresignedDownloadUrl(string key, int expiresInMinutes);
        Task DeleteFileAsync(string key);
    }

    public class S3FileService : IS3FileService
    {
        private readonly IAmazonS3 _s3Client;
        private const string BucketName = "examai-exams-storage"; // שם הבאקט שלכם

        public S3FileService()
        {
            // במציאות ה-S3Client מוזרק דרך ה-DI ומקבל הגדרות מ-appsettings
            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1); 
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string userId, string examId, string type)
        {
            // בניית הקידומת המובנית (Structured Path) כפי שנדרש
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string s3Key = $"{userId}/{examId}/{type}/{timestamp}_{fileName}";

            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = s3Key,
                InputStream = fileStream,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(putRequest);
            return s3Key;
        }

        public string GetPresignedDownloadUrl(string key, int expiresInMinutes)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes)
            };

            return _s3Client.GetPreSignedURL(request);        }

        public async Task DeleteFileAsync(string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
    }
}