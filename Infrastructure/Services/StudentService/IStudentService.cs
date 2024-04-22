using Domain.DTOs.StudentDTO;

namespace Infrastructure.Services.StudentService;

public interface IStudentService
{
    Task<string> AddStudent(AddStudentDto student);
}