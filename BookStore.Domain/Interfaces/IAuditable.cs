namespace BookStore.Domain.Interfaces;

public interface IAuditable
{
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}