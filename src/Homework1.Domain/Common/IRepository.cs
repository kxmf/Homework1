namespace Homework1.Domain.Common;

public interface IRepository<TAggregate, in TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull, IEquatable<TId>
{
}
