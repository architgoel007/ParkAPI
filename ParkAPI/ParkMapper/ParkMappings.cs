using AutoMapper;
using ParkAPI.Models;
using ParkAPI.Models.Dtos;

namespace ParkAPI.ParkMapper
{
    public class ParkMappings : Profile
    {
        public ParkMappings()
        {
            CreateMap<NationalPark, NationalParkDTO>().ReverseMap();
        }
    }
}
