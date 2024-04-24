namespace Domain.Filter;

public class StudentFilter:PaginationFilter
{
    public string? Address { get; set; }
    public string? Email { get; set; }
}