using System.Text.Json;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure.Persistance.Interceptors;

public class AuditLogInterceptor(
    ICurrentUser userService, 
    IServiceProvider serviceProvider
    ) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        // 1. Detect Changes
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable or ISoftDeletable)
            .Where(e => e.State == EntityState.Added || 
                        e.State == EntityState.Modified || 
                        e.State == EntityState.Deleted)
            .ToList();

        if (!entries.Any()) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var logs = new List<AuditLog>();
        var timestamp = DateTime.UtcNow;
        var userId = userService.Id;

        foreach (var entry in entries)
    {
        var actionName = entry.State.ToString();
        
        // =========================================================
        // STEP 1: HANDLE SOFT DELETE 
        // (Must happen first because it changes the State!)
        // =========================================================
        var isSoftDeleteAction = false;
        
        if (entry is { State: EntityState.Deleted, Entity: ISoftDeletable softDeletable })
        {
            // 1. Change State to Modified (Stop physical delete)
            entry.State = EntityState.Modified;
            
            // 2. Set Flags
            softDeletable.IsDeleted = true;
            softDeletable.DeletedAt = timestamp;
            
            // 3. Mark internal flag for the Auditor below
            isSoftDeleteAction = true;
            actionName = "Deleted"; // Override "Modified" to "Deleted" for the log
        }

        // =========================================================
        // STEP 2: HANDLE AUDITING
        // (Only if the entity specifically supports IAuditable)
        // =========================================================
        if (entry.Entity is IAuditable)
        {
            var changes = new Dictionary<string, object>();

            // Scenario A: Soft Delete (We detected it above)
            if (isSoftDeleteAction)
            {
                changes["IsDeleted"] = new { Old = false, New = true };
            }
            // Scenario B: Standard Modification
            else if (entry.State == EntityState.Modified)
            {
                foreach (var prop in entry.Properties)
                {
                    if (prop.IsModified)
                    {
                        changes[prop.Metadata.Name] = new
                        {
                            Old = prop.OriginalValue,
                            New = prop.CurrentValue
                        };
                    }
                }
            }
            // Scenario C: Added / Deleted (Hard)
            else
            {
                // Logic for Added: maybe log all properties or just "Created"
                // Logic for Hard Delete: log ID
            }

            // Create Log
            logs.Add(new AuditLog 
            {
                EntityId = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue?.ToString() ?? "Unknown",
                EntityType = entry.Entity.GetType().Name,
                Action = actionName,
                Timestamp = timestamp,
                Changes = System.Text.Json.JsonSerializer.Serialize(changes),
                UserId = userId
            });
        }
    }

        // 3. Delegate Storage to the Sink
        if (logs.Count != 0)
        {
            // The DbContext is fully built, so resolving the Sink (which needs DbContext) won't freeze.
            var auditSink = serviceProvider.GetRequiredService<IAuditLogSink>();
            await auditSink.WriteLogsAsync(logs, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}