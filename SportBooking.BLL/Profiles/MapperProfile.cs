using AutoMapper;
using SportBooking.BLL.Dtos;
using SportBooking.DAL.Entities;

namespace SportBooking.BLL.Profiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<SportField, SportFieldDto>()
            .ForMember(dest => dest.SportFieldDetailId, 
                t => t.MapFrom(src => src.SportFieldDetail.Id))
            .ForMember(dest => dest.EndProgram,
                t => t.MapFrom(src => src.SportFieldDetail.EndProgram))
            .ForMember(dest => dest.StartProgram,
                t => t.MapFrom(src => src.SportFieldDetail.StartProgram))
            .ForMember(dest => dest.Description,
                t => t.MapFrom(src => src.SportFieldDetail.Description))
            .ForMember(dest => dest.Address, t => t.MapFrom(src => src.SportFieldDetail.Address))
            .ReverseMap();
        CreateMap<Reservation, ReservationDto>();
        CreateMap<ReservationDto, Reservation>();
    }
}