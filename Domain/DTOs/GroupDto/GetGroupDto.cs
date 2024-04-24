using Domain.Enums;

namespace Domain.DTOs.GroupDto;

public class GetGroupDto
{
    public int Id { get; set; }
    public string GroupName { get; set; } = null!;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public int CourseId { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}