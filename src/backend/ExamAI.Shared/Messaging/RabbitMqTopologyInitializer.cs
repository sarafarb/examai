using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ExamAI.Shared.Messaging
{
    public class RabbitMqTopologyInitializer
    {
        private readonly IConfiguration _configuration;

        public RabbitMqTopologyInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // --- 1. Define Exchanges ---
            channel.ExchangeDeclare("examai.events", ExchangeType.Topic, durable: true);
            channel.ExchangeDeclare("examai.jobs", ExchangeType.Direct, durable: true);
            channel.ExchangeDeclare("examai.dlx", ExchangeType.Direct, durable: true);

            // הגדרות ברירת מחדל לתורים (שעה TTL)
            var commonArgs = new Dictionary<string, object> { { "x-message-ttl", 3600000 } };

            // --- 2. Define Queues ---
            
            // ocr.jobs (with DLX)
            var ocrArgs = new Dictionary<string, object>
            {
                { "x-message-ttl", 3600000 },
                { "x-dead-letter-exchange", "examai.dlx" },
                { "x-dead-letter-routing-key", "job.ocr.dlq" }
            };
            channel.QueueDeclare("ocr.jobs", durable: true, exclusive: false, autoDelete: false, arguments: ocrArgs);

            // grading.jobs (with DLX)
            var gradingArgs = new Dictionary<string, object>
            {
                { "x-message-ttl", 3600000 },
                { "x-dead-letter-exchange", "examai.dlx" },
                { "x-dead-letter-routing-key", "job.grading.dlq" }
            };
            channel.QueueDeclare("grading.jobs", durable: true, exclusive: false, autoDelete: false, arguments: gradingArgs);

            // Standard Queues
            channel.QueueDeclare("export.jobs", durable: true, exclusive: false, autoDelete: false, arguments: commonArgs);
            channel.QueueDeclare("notification.jobs", durable: true, exclusive: false, autoDelete: false, arguments: commonArgs);

            // Dead Letter Queues (DLQs)
            channel.QueueDeclare("ocr.jobs.dlq", durable: true, exclusive: false, autoDelete: false, arguments: commonArgs);
            channel.QueueDeclare("grading.jobs.dlq", durable: true, exclusive: false, autoDelete: false, arguments: commonArgs);

            // --- 3. Define Bindings & Routing Keys ---
            channel.QueueBind("ocr.jobs", "examai.jobs", "job.ocr");
            channel.QueueBind("grading.jobs", "examai.jobs", "job.grading");
            channel.QueueBind("export.jobs", "examai.jobs", "job.export");
            channel.QueueBind("notification.jobs", "examai.jobs", "job.notification");

            // DLQ Bindings
            channel.QueueBind("ocr.jobs.dlq", "examai.dlx", "job.ocr.dlq");
            channel.QueueBind("grading.jobs.dlq", "examai.dlx", "job.grading.dlq");
        }
    }
}