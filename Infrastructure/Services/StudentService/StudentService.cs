using AutoMapper;
using Domain.DTOs.StudentDTO;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Services.StudentService;

public class StudentService:IStudentService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public StudentService(DataContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<string> AddStudent(AddStudentDto student)
    {
        var mapped = _mapper.Map<Student>(student);

        await _context.Students.AddAsync(mapped);
        await _context.SaveChangesAsync();
        return "Success";
    }
}