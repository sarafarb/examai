using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ExamAI.Shared.Messaging
{
    public abstract class IMessageConsumer<T> : IDisposable
    {
        protected readonly IConnection Connection;
        protected readonly IModel Channel;
        protected readonly string QueueName;

        protected IMessageConsumer(string hostName, string queueName)
        {
            QueueName = queueName;
            var factory = new ConnectionFactory() { HostName = hostName, DispatchConsumersAsync = true };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
        }

        public void StartConsuming()
        {
            var consumer = new AsyncEventingBasicConsumer(Channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                
                try
                {
                    var message = JsonSerializer.Deserialize<T>(json);
                    await ConsumeAsync(message);
                    Channel.BasicAck(ea.DeliveryTag, false); // אישור קבלה מוצלח
                }
                catch (Exception)
                {
                    // במקרה של שגיאה - דוחף ל-Dead Letter Exchange (במידה והוגדר לתור)
                    Channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            Channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        }

        // פונקציה שכל Worker ייאלץ לממש בעצמו עם הלוגיקה שלו
        protected abstract Task ConsumeAsync(T message);

        public void Dispose()
        {
            Channel?.Close();
            Connection?.Close();
        }
    }
}