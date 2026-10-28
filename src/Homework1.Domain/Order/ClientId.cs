using Homework1.Domain.Common;
using System.Diagnostics.CodeAnalysis;

namespace Homework1.Domain.Order;

public readonly record struct ClientId : IStronglyTypedId<long, ClientId>
{
    public long Value { get; }
    private ClientId(long value) => Value = value;

    public static ClientId Empty => default;
    public bool IsEmpty => Value == 0;
    public static ClientId Create(long value)
    {
        if (!TryCreate(value, out var result))
        {
            throw new InvalidIdException($"ClientId must be a positive number greater than zero, but was '{value}'.");
        }
        return result;
    }

    public static bool TryCreate(long value, [NotNullWhen(true)] out ClientId result)
    {
        if (value <= 0)
        {
            result = default;
            return false;
        }

        result = new ClientId(value);
        return true;
    }

    public override string ToString() => Value.ToString();
}
