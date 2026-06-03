using MediatR;
namespace ExamAI.Shared.Domain;
public interface IDomainEvent : INotification { DateTime OccurredOn { get; } }
