using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces;

public interface IAuditLogSink
{
    Task WriteLogsAsync(IEnumerable<AuditLog> logs, CancellationToken cancellationToken = default);
}