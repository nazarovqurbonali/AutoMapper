using System.Net;
using Domain.DTOs.GroupDto;
using Domain.DTOs.StudentDTO;
using Domain.Entities;
using Domain.Filter;
using Domain.Responses;
using Infrastructure.Services.StudentService;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("[controller]")]
[ApiController]
public class StudentController:ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("get-students")]
    public async Task<PagedResponse<List<GetStudentDto>>> GetStudentsAsync([FromQuery]StudentFilter filter)
    {
        return await _studentService.GetStudentsAsync(filter);
        // return new Response<List<GetStudentDto>>(HttpStatusCode.OK,"Success");
    }
    
    [HttpGet("get-groups-with-count-student")]
    public async Task<Response<List<GroupWithCountOfStudentDto>>> GetGroupWithCountStudent( )
    {
        return await _studentService.GetStudentWithCountOfStudentDtoAsync();
        // return new Response<List<GetStudentDto>>(HttpStatusCode.OK,"Success");
    }
    
    [HttpGet("{studentId:int}")]
    public async Task<Response<GetStudentDto>> GetStudentByIdAsync([FromBody]int studentId)
    {
        return await _studentService.GetStudentByIdAsync(studentId);
    }
    
    [HttpPost("create-student")]
    public async Task<Response<string>> AddStudentAsync([FromBody]AddStudentDto studentDto)
    {
        return await _studentService.CreateStudentAsync(studentDto);
    }
    
    [HttpPut("update-student")]
    public async Task<Response<string>> UpdateStudentAsync([FromBody]UpdateStudentDto studentDto)
    {
        return await _studentService.UpdateStudentAsync(studentDto);
    }
    
    [HttpDelete("{studentId:int}")]
    public async Task<Response<bool>> DeleteStudentAsync([FromBody]int studentId)
    {
        return await _studentService.DeleteStudentAsync(studentId);
    }

    // [HttpOptions]
    // public  IActionResult Test()
    // {
    //     return StatusCode(200, "Student");
    // }
    //
}