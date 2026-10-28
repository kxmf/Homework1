using Homework1.Domain.Common;
using System.Diagnostics.CodeAnalysis;

namespace Homework1.Domain.Order;

public readonly record struct OrderId : IStronglyTypedId<long, OrderId>
{
    public long Value { get; }

    private OrderId(long value) => Value = value;
    public static OrderId Empty => default;
    public bool IsEmpty => Value == 0;

    public static OrderId Create(long value)
    {
        if (!TryCreate(value, out var result))
        {
            throw new InvalidIdException($"OrderId must be a positive number greater than zero, but was '{value}'.");
        }

        return result;
    }

    public static bool TryCreate(long value, [NotNullWhen(true)] out OrderId result)
    {
        if (value <= 0)
        {
            result = default;
            return false;
        }

        result = new OrderId(value);
        return true;
    }

    public override string ToString() => Value.ToString();
}
