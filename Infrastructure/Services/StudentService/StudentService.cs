using System.ComponentModel;
using System.Data.Common;
using System.Net;
using AutoMapper;
using Domain.DTOs;
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
            
            
            var query = (from s in _context.Students
                join sg in _context.StudentGroups on s.Id equals sg.StudentId
                join g in _context.Groups on sg.GroupId equals g.Id
                join c in _context.Courses on g.CourseId equals c.Id
                group g by new { s.FirstName, Course = c } into res
                select new ExampleDto()
                {
                    StudentName = res.Key.FirstName ,
                    Groups = res.ToList(),
                    Course = res.Key.Course
                });
            
            
            
            
            
            

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

    public async Task<Response<List<GetStudentDto>>> GetStudents()
    {
        try
        {
            
            var query2= from s in _context.Students
                let countAt= _context.ProgressBooks.Count(x=>x.StudentId==s.Id && x.IsAttended==false)
                    where countAt==0
                    select new GetStudentDto
              {
                 
             }
            
            
            
            
            string filter = ".NET";
            var query1 = await (from s in _context.Students
                join sg in _context.StudentGroups on s.Id equals sg.StudentId
                join g in _context.Groups on sg.GroupId equals g.Id
                join c in _context.Courses on g.CourseId equals c.Id
                where c.CourseName == filter
                select new
                {
                    Student = s
                }).ToListAsync();

            var map = _mapper.Map<List<GetStudentDto>>(query1);
            
            return new Response<List<GetStudentDto>>(map);

        }
        catch (Exception e)
        {
            return new Response<List<GetStudentDto>>(HttpStatusCode.InternalServerError, e.Message);
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

    public async Task<Response<string>> CreateTimeTableAsync(AddTimeTableDto timeTable)
    {
        try
        {
            var newTimeTable = new TimeTable()
            {
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                FromTime = TimeSpan.Parse(timeTable.FromTime),
                ToTime = TimeSpan.Parse(timeTable.ToTime),
                DayOfWeek = timeTable.DayOfWeek
            };

            await _context.TimeTables.AddAsync(newTimeTable);
            await _context.SaveChangesAsync();

            return new Response<string>("Success");
        }
        catch (DbException dbException)
        {
            return new Response<string>(HttpStatusCode.InternalServerError, dbException.Message);
        }
        catch (Exception exception)
        {
            return new Response<string>(HttpStatusCode.InternalServerError, exception.Message);
        }
    }

    #endregion
}


public class ExampleDto
{
    public string? StudentName { get; set; }
    public int? CountGroup { get; set; }
    public List<Group>? Groups { get; set; }
    public Course? Course { get; set; }
}