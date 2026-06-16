using System.Threading.Tasks;

namespace ExamAI.Shared.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string exchange, string routingKey, T message);
    }
}