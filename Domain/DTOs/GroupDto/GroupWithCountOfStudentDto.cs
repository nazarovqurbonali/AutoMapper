using Domain.Entities;

namespace Domain.DTOs.GroupDto;

public class GroupWithCountOfStudentDto
{
    public Group? Group { get; set; }
    public int CountOfStudents { get; set; }
}