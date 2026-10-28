using System.Diagnostics.CodeAnalysis;

namespace Homework1.Domain.Common;

public interface IStronglyTypedId<TValue, TSelf> : IEquatable<TSelf>
    where TValue : notnull
    where TSelf : IStronglyTypedId<TValue, TSelf>
{
    TValue Value { get; }

    static abstract TSelf Create(TValue value);

    static abstract bool TryCreate(TValue value, [NotNullWhen(true)] out TSelf? result);
}
