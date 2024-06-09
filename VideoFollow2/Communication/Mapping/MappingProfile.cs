using AutoMapper;
using Communication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Mapping
{
    public class MappingProfile : Profile
    {


        public MappingProfile() 
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, ProfileDTO>().ReverseMap();
            CreateMap<User, UserResponseDto>().ReverseMap();
            CreateMap<Ride, RideRequestDTO>().ReverseMap();
            CreateMap<Ride, RideResponseDTO>().ReverseMap();
            CreateMap<Ride, RideTableDTO>().ReverseMap();
            CreateMap<Ride, CountdownDTO>().ReverseMap();
            CreateMap<Ride, RatingDTO>().ReverseMap();

        }


    }
}
