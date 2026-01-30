namespace BookStore.Application.Common;

public class PaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "asc"; // asc or desc
    public string? SearchQuery { get; set; }
}