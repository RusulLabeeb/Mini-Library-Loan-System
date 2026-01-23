using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;

namespace BookStore.Infrastructure.Services;

public class EfCoreAuditLogSink(IBookStoreDbContext context) : IAuditLogSink
{
    public async Task WriteLogsAsync(IEnumerable<AuditLog> logs, CancellationToken cancellationToken)
    {
        // Since this runs inside "SavingChangesAsync", these will be committed 
        // along with the main transaction automatically and atomically.
        // NOTE!! We shouldn't call SaveChangesAsync here again it will cause a recursion of the interceptor.
        await context.AuditLogs.AddRangeAsync(logs, cancellationToken);
    }
}