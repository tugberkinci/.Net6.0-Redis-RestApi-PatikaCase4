using AutoMapper;
using PatikaHomework4.Data.Model;
using PatikaHomework4.Dto.Dto;

namespace PatikaHomework4.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PersonDto, Person>().ReverseMap();
        }
     
    }
}
