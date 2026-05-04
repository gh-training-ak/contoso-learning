namespace Contoso.Application.Abstractions;

public interface IAuditSink
{
    Task WriteAsync(AuditEntry entry, CancellationToken ct = default);
}

public sealed record AuditEntry(
    Guid Id,
    string Actor,
    string Action,
    string ResourceType,
    string ResourceId,
    DateTimeOffset OccurredAt,
    IReadOnlyDictionary<string, string?> Changes)
{
    public static AuditEntry For(string actor, string action, string resourceType, Guid resourceId,
        DateTimeOffset when, IReadOnlyDictionary<string, string?>? changes = null)
        => new(Guid.NewGuid(), actor, action, resourceType, resourceId.ToString(), when,
               changes ?? new Dictionary<string, string?>());
}
