using AutoMapper;
using Model.Models;
using Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dtos;

internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskDto, TaskModel>().ReverseMap();
        CreateMap<SubTaskDto, SubTaskModel>().ReverseMap();
    }
}
