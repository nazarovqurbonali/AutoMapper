using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.StudentDTO;
using Domain.Entities;

namespace Infrastructure.AutoMapper;

public class MapperProfile:Profile
{
    public MapperProfile()
    {
        CreateMap<Student, AddStudentDto>().ReverseMap();
        CreateMap<Student, GetStudentDto>().ReverseMap();
        CreateMap<Student, UpdateStudentDto>().ReverseMap();
        
        //ForMembers
        CreateMap<AddTimeTableDto, TimeTable>()
            .ForMember(en => en.FromTime, dto=> dto.MapFrom(s => TimeSpan.Parse(s.FromTime)))
            .ForMember(x => x.ToTime, opt
                => opt.MapFrom(x => TimeSpan.Parse(x.ToTime)));


        // //Reverse map
        // CreateMap<BaseStudentDto,Student>().ReverseMap();
        //
        // // ignore
        // CreateMap<Student, AddStudentDto>()
        //     .ForMember(dest => dest.FirstName, opt => opt.Ignore());

        // CreateMap<Course,AddTimeTableDto>()
        //     .ForMember(x=>x.CourseN,y=>y.MapFrom(x=>x.CourseName))
        //     .ForMember(x=>x.Desc,y=>y.Ignore())

    }   
}