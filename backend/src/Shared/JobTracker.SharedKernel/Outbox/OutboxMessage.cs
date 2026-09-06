namespace JobTracker.SharedKernel.Outbox;

/// <summary>
/// Persisted in the same transaction as the aggregate change that raised the domain
/// event(s) — see <see cref="InsertOutboxMessagesInterceptor"/>. Each module maps this
/// to its own outbox_messages table within its own schema (jobs.outbox_messages,
/// billing.outbox_messages, ...), so modules never share a physical outbox table.
/// </summary>
public sealed class OutboxMessage
{
    public required Guid Id { get; init; }

    public required string Type { get; init; }

    public required string Content { get; init; }

    public required DateTime OccurredOnUtc { get; init; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
}
