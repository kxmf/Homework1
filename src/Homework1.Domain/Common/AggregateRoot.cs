namespace Homework1.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull, IEquatable<TId>
{
}
