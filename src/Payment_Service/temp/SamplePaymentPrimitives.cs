using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Payment_Service.Temp.Generated;

// In-memory sample types. These do not connect to payment providers.
public readonly record struct SampleMoney(decimal Amount, string Currency)
{
    public static SampleMoney Create(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }
        var normalized = currency.Trim().ToUpperInvariant();
        if (normalized.Length != 3 || normalized.Any(c => c < 'A' || c > 'Z'))
        {
            throw new ArgumentException("Use a three-letter currency code.", nameof(currency));
        }
        return new SampleMoney(amount, normalized);
    }

    public SampleMoney Add(SampleMoney other)
    {
        EnsureCurrency(other);
        return Create(checked(Amount + other.Amount), Currency);
    }

    public SampleMoney Subtract(SampleMoney other)
    {
        EnsureCurrency(other);
        return Create(checked(Amount - other.Amount), Currency);
    }

    public SampleMoney Multiply(decimal multiplier)
    {
        if (multiplier < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier));
        }
        return Create(checked(Amount * multiplier), Currency);
    }

    public SampleMoney Round(int decimalPlaces)
    {
        if (decimalPlaces < 0 || decimalPlaces > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
        }
        return Create(decimal.Round(Amount, decimalPlaces, MidpointRounding.ToEven), Currency);
    }

    private void EnsureCurrency(SampleMoney other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Currency mismatch.");
        }
    }
}

public interface ISamplePaymentSnapshot
{
    Guid Id { get; }
    Guid OwnerId { get; }
    SampleMoney Money { get; }
    string Reference { get; }
    string Description { get; }
    string State { get; }
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }
    long Version { get; }
}

public sealed record SampleAuditEntry(
    Guid Id,
    Guid ResourceId,
    string ResourceType,
    string Action,
    string Actor,
    string PreviousState,
    string CurrentState,
    long Version,
    DateTimeOffset OccurredAt,
    string Note);

public sealed record SampleCurrencySummary(
    string Currency,
    int Count,
    decimal Total,
    decimal Minimum,
    decimal Maximum);

public sealed record SampleQuery
{
    public Guid? OwnerId { get; init; }
    public string? Currency { get; init; }
    public string? State { get; init; }
    public string? Search { get; init; }
    public decimal? MinimumAmount { get; init; }
    public decimal? MaximumAmount { get; init; }
    public DateTimeOffset? CreatedFrom { get; init; }
    public DateTimeOffset? CreatedUntil { get; init; }
    public int Offset { get; init; }
    public int Limit { get; init; } = 25;
    public bool OldestFirst { get; init; }

    public void Validate()
    {
        if (Offset < 0 || Limit < 1 || Limit > 200)
        {
            throw new ArgumentOutOfRangeException(nameof(Limit), "Invalid page bounds.");
        }
        if (MinimumAmount.HasValue && MaximumAmount.HasValue && MinimumAmount > MaximumAmount)
        {
            throw new ArgumentException("Minimum amount exceeds maximum amount.");
        }
        if (CreatedFrom.HasValue && CreatedUntil.HasValue && CreatedFrom > CreatedUntil)
        {
            throw new ArgumentException("Start date exceeds end date.");
        }
    }

    public bool Matches(ISamplePaymentSnapshot item)
    {
        if (OwnerId.HasValue && item.OwnerId != OwnerId.Value)
        {
            return false;
        }
        if (!string.IsNullOrWhiteSpace(Currency) &&
            !string.Equals(item.Money.Currency, Currency.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        if (!string.IsNullOrWhiteSpace(State) &&
            !string.Equals(item.State, State.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        if (MinimumAmount.HasValue && item.Money.Amount < MinimumAmount.Value)
        {
            return false;
        }
        if (MaximumAmount.HasValue && item.Money.Amount > MaximumAmount.Value)
        {
            return false;
        }
        if (CreatedFrom.HasValue && item.CreatedAt < CreatedFrom.Value)
        {
            return false;
        }
        if (CreatedUntil.HasValue && item.CreatedAt > CreatedUntil.Value)
        {
            return false;
        }
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = Search.Trim();
            return item.Reference.Contains(term, StringComparison.OrdinalIgnoreCase)
                || item.Description.Contains(term, StringComparison.OrdinalIgnoreCase);
        }
        return true;
    }
}

public sealed record SamplePage<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Offset,
    int Limit)
{
    public bool HasPrevious => Offset > 0;
    public bool HasNext => Offset + Items.Count < Total;
}

public static class SampleGuard
{
    public static string Text(string value, string name, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", name);
        }
        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException("Value exceeds the allowed length.", name);
        }
        return normalized;
    }

    public static void Owner(Guid ownerId)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner identifier is required.", nameof(ownerId));
        }
    }

    public static void Version(long actual, long expected)
    {
        if (actual != expected)
        {
            throw new InvalidOperationException("The sample was modified. Reload its version.");
        }
    }

    public static IReadOnlyDictionary<string, string> Metadata(
        IReadOnlyDictionary<string, string>? source)
    {
        var copy = new Dictionary<string, string>(StringComparer.Ordinal);
        if (source is not null)
        {
            if (source.Count > 32)
            {
                throw new ArgumentException("At most 32 metadata entries are supported.");
            }
            foreach (var entry in source)
            {
                var key = Text(entry.Key, "metadata key", 64);
                var value = Text(entry.Value, "metadata value", 512);
                copy.Add(key, value);
            }
        }
        return new ReadOnlyDictionary<string, string>(copy);
    }
}
