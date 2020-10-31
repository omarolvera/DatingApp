using System;
using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl, opt => 
            opt.MapFrom(src => src.Photos.FirstOrDefault(p=> p.IsMain).Url))
            .ForMember(u => u.Age, opt => opt.MapFrom(b => b.DateOfBirth.CalculateAge() ));
            CreateMap<User, UserDetailDto>()
             .ForMember(u => u.Age, opt => opt.MapFrom(b => b.DateOfBirth.CalculateAge()))
            .ForMember(dest => dest.PhotoUrl, opt => 
             opt.MapFrom(src => src.Photos.FirstOrDefault(p=> p.IsMain).Url));
            CreateMap<Photo, PhotoDto>();

            CreateMap<UserForUpdateDto, User>();
        }

      
    }

}