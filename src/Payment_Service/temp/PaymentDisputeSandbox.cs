using System;
using System.Collections.Generic;
using System.Linq;

namespace Payment_Service.Temp.Generated;

public enum PaymentDisputeStage
{
    Opened,
    Reviewing,
    Resolved,
    Rejected
}

public sealed record PaymentDisputeCreateRequest(
    Guid OwnerId,
    decimal Amount,
    string Currency,
    string Reference,
    string Description,
    IReadOnlyDictionary<string, string>? Metadata = null);

public sealed record PaymentDisputeSnapshot(
    Guid Id,
    Guid OwnerId,
    SampleMoney Money,
    string Reference,
    string Description,
    PaymentDisputeStage Stage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    long Version,
    IReadOnlyDictionary<string, string> Metadata) : ISamplePaymentSnapshot
{
    public string State => Stage.ToString();
    public bool IsTerminal => Stage is PaymentDisputeStage.Resolved or PaymentDisputeStage.Rejected;
}

// Local sample workflow: review a payment dispute.
public sealed class PaymentDisputeSandbox
{
    private readonly object _sync = new();
    private readonly Dictionary<Guid, PaymentDisputeSnapshot> _items = new();
    private readonly List<SampleAuditEntry> _audit = new();
    private readonly Func<DateTimeOffset> _utcNow;

    public PaymentDisputeSandbox(Func<DateTimeOffset>? utcNow = null)
    {
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
    }

    public PaymentDisputeSnapshot Create(PaymentDisputeCreateRequest request, string actor)
    {
        ArgumentNullException.ThrowIfNull(request);
        SampleGuard.Owner(request.OwnerId);
        actor = SampleGuard.Text(actor, nameof(actor), 120);
        var money = SampleMoney.Create(request.Amount, request.Currency);
        var reference = SampleGuard.Text(request.Reference, nameof(request.Reference), 100);
        var description = SampleGuard.Text(request.Description, nameof(request.Description), 500);
        var metadata = SampleGuard.Metadata(request.Metadata);
        lock (_sync)
        {
            if (_items.Values.Any(x => x.OwnerId == request.OwnerId &&
                string.Equals(x.Reference, reference, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Reference already exists for this owner.");
            }
            var now = _utcNow();
            var item = new PaymentDisputeSnapshot(
                Guid.NewGuid(), request.OwnerId, money, reference, description,
                PaymentDisputeStage.Opened, now, now, 1, metadata);
            _items.Add(item.Id, item);
            AppendAudit(item, "create", actor, string.Empty, "Sample record created.");
            return item;
        }
    }

    public PaymentDisputeSnapshot? Find(Guid id)
    {
        lock (_sync)
        {
            return _items.GetValueOrDefault(id);
        }
    }

    public PaymentDisputeSnapshot? FindByReference(Guid ownerId, string reference)
    {
        reference = SampleGuard.Text(reference, nameof(reference), 100);
        lock (_sync)
        {
            return _items.Values.FirstOrDefault(x => x.OwnerId == ownerId &&
                string.Equals(x.Reference, reference, StringComparison.OrdinalIgnoreCase));
        }
    }

    public SamplePage<PaymentDisputeSnapshot> List(SampleQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        query.Validate();
        lock (_sync)
        {
            var matches = _items.Values.Where(x => query.Matches(x)).ToArray();
            var ordered = query.OldestFirst
                ? matches.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id)
                : matches.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id);
            var page = ordered.Skip(query.Offset).Take(query.Limit).ToArray();
            return new SamplePage<PaymentDisputeSnapshot>(page, matches.Length, query.Offset, query.Limit);
        }
    }

    public PaymentDisputeSnapshot MoveTo(
        Guid id, PaymentDisputeStage stage, long expectedVersion, string actor, string? note = null)
    {
        actor = SampleGuard.Text(actor, nameof(actor), 120);
        var auditNote = note is null ? "Sample stage changed." : SampleGuard.Text(note, nameof(note), 500);
        lock (_sync)
        {
            var current = Require(id);
            SampleGuard.Version(current.Version, expectedVersion);
            if (!CanTransition(current.Stage, stage))
            {
                throw new InvalidOperationException($"Cannot move from {current.Stage} to {stage}.");
            }
            var updated = current with
            {
                Stage = stage,
                UpdatedAt = _utcNow(),
                Version = checked(current.Version + 1)
            };
            _items[id] = updated;
            AppendAudit(updated, "transition", actor, current.State, auditNote);
            return updated;
        }
    }

    public PaymentDisputeSnapshot AmendDescription(
        Guid id, string description, long expectedVersion, string actor)
    {
        description = SampleGuard.Text(description, nameof(description), 500);
        actor = SampleGuard.Text(actor, nameof(actor), 120);
        lock (_sync)
        {
            var current = Require(id);
            SampleGuard.Version(current.Version, expectedVersion);
            if (current.IsTerminal)
            {
                throw new InvalidOperationException("A terminal sample cannot be amended.");
            }
            var updated = current with
            {
                Description = description,
                UpdatedAt = _utcNow(),
                Version = checked(current.Version + 1)
            };
            _items[id] = updated;
            AppendAudit(updated, "amend", actor, current.State, "Sample description amended.");
            return updated;
        }
    }

    public IReadOnlyList<SampleAuditEntry> GetAudit(Guid id)
    {
        lock (_sync)
        {
            Require(id);
            return _audit.Where(x => x.ResourceId == id).OrderBy(x => x.Version).ToArray();
        }
    }

    public IReadOnlyList<SampleCurrencySummary> Summarize(Guid? ownerId = null)
    {
        lock (_sync)
        {
            return _items.Values
                .Where(x => !ownerId.HasValue || x.OwnerId == ownerId.Value)
                .GroupBy(x => x.Money.Currency)
                .Select(group => new SampleCurrencySummary(
                    group.Key, group.Count(), group.Sum(x => x.Money.Amount),
                    group.Min(x => x.Money.Amount), group.Max(x => x.Money.Amount)))
                .OrderBy(x => x.Currency)
                .ToArray();
        }
    }

    public static bool CanTransition(PaymentDisputeStage from, PaymentDisputeStage to)
    {
        return (from, to) switch
        {
            (PaymentDisputeStage.Opened, PaymentDisputeStage.Reviewing) => true,
            (PaymentDisputeStage.Opened, PaymentDisputeStage.Rejected) => true,
            (PaymentDisputeStage.Reviewing, PaymentDisputeStage.Resolved) => true,
            (PaymentDisputeStage.Reviewing, PaymentDisputeStage.Rejected) => true,
            _ => false
        };
    }

    private PaymentDisputeSnapshot Require(Guid id)
    {
        if (!_items.TryGetValue(id, out var item))
        {
            throw new KeyNotFoundException($"Sample {id} was not found.");
        }
        return item;
    }

    private void AppendAudit(
        PaymentDisputeSnapshot item, string action, string actor, string previousState, string note)
    {
        _audit.Add(new SampleAuditEntry(
            Guid.NewGuid(), item.Id, "PaymentDispute", action, actor,
            previousState, item.State, item.Version, item.UpdatedAt, note));
    }
}
