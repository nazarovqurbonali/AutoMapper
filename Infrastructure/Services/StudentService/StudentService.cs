using System.Data.Common;
using System.Net;
using AutoMapper;
using Domain.DTOs.GroupDto;
using Domain.DTOs.StudentDTO;
using Domain.Entities;
using Domain.Filter;
using Domain.Responses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.StudentService;

public class StudentService : IStudentService
{
    #region ctor

    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public StudentService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    

    #endregion

    #region GetStudentsAsync

    public async Task<PagedResponse<List<GetStudentDto>>> GetStudentsAsync(StudentFilter filter)
    {
        try
        {
            var students = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Address))
                students = students.Where(x => x.Address.ToLower().Contains(filter.Address.ToLower()));
            if (!string.IsNullOrEmpty(filter.Email))
                students = students.Where(x => x.Email.ToLower().Contains(filter.Email.ToLower()));

            var response = await students
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize).ToListAsync();
            var totalRecord = students.Count();

            var mapped = _mapper.Map<List<GetStudentDto>>(response);
            return new PagedResponse<List<GetStudentDto>>(mapped, filter.PageNumber, filter.PageSize, totalRecord);

        }
        catch (DbException dbEx)
        {
            return new PagedResponse<List<GetStudentDto>>(HttpStatusCode.InternalServerError, dbEx.Message);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<GetStudentDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<List<GroupWithCountOfStudentDto>>> GetStudentWithCountOfStudentDtoAsync()
    {
        try
        {
            var existing = await (from g in _context.Groups
                let count = _context.StudentGroups.Count(x => x.GroupId == g.Id)
                select new GroupWithCountOfStudentDto
                {
                    Group = g,
                    CountOfStudents = count
                }).ToListAsync();
            return new Response<List<GroupWithCountOfStudentDto>>(existing);
        }
        catch (Exception e)
        {
            return new Response<List<GroupWithCountOfStudentDto>>(HttpStatusCode.InternalServerError,e.Message);
        }
    }

    #endregion

    #region GetStudentByIdAsync

    public async Task<Response<GetStudentDto>> GetStudentByIdAsync(int id)
    {
        try
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return new Response<GetStudentDto>(HttpStatusCode.BadRequest, "Student not found");
            var mapped = _mapper.Map<GetStudentDto>(student);
            return new Response<GetStudentDto>(mapped);
        }
        catch (Exception e)
        {
            return new Response<GetStudentDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreateStudentAsync

    public async Task<Response<string>> CreateStudentAsync(AddStudentDto student)
    {
        try
        {
            var existingStudent = await _context.Students.FirstOrDefaultAsync(x => x.Email == student.Email);
            if (existingStudent != null)
                return new Response<string>(HttpStatusCode.BadRequest, "Student already exists");
            var mapped = _mapper.Map<Student>(student);

            await _context.Students.AddAsync(mapped);
            await _context.SaveChangesAsync();

            return new Response<string>("Successfully created a new student");
        }
        catch (Exception e)
        {
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region UpdateStudentAsync

    public async Task<Response<string>> UpdateStudentAsync(UpdateStudentDto student)
    {
        try
        {
            var mappedStudent = _mapper.Map<Student>(student);
            _context.Students.Update(mappedStudent);
            var update= await _context.SaveChangesAsync();
            if(update==0)  return new Response<string>(HttpStatusCode.BadRequest, "Student not found");
            return new Response<string>("Student updated successfully");
        }
        catch (Exception e)
        {
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region DeleteStudentAsync

    public async Task<Response<bool>> DeleteStudentAsync(int id)
    {
        try
        {
            var student = await _context.Students.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (student == 0)
                return new Response<bool>(HttpStatusCode.BadRequest, "Student not found");

            return new Response<bool>(true);
        }
        catch (Exception e)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion
}

