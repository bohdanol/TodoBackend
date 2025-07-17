using AutoMapper;
using Model.Dtos;
using Model.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskDto, TaskModel>().ReverseMap();
        CreateMap<SubTaskDto, SubTaskModel>().ReverseMap();
    }
}
