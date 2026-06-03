namespace ExamAI.Shared.Domain;
public abstract class Entity<TId> { public TId Id { get; protected set; } = default!; }
