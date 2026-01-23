using System.ComponentModel.DataAnnotations;

namespace BookStore.Domain.Entities;

public class AuditLog
{
    [Key]
    public Guid Id { get; set; }
    public int? UserId { get; set; }     // Nullable for system actions
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string Action { get; set; }
    public string Changes { get; set; }
    public DateTime Timestamp { get; set; }
}