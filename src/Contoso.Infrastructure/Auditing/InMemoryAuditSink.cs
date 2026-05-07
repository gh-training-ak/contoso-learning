using System.Collections.Concurrent;
using Contoso.Application.Abstractions;

namespace Contoso.Infrastructure.Auditing;

public sealed class InMemoryAuditSink : IAuditSink
{
    private readonly ConcurrentQueue<AuditEntry> _entries = new();

    public IReadOnlyCollection<AuditEntry> Entries => _entries.ToArray();

    public Task WriteAsync(AuditEntry entry, CancellationToken ct = default)
    {
        _entries.Enqueue(entry);
        return Task.CompletedTask;
    }
}
