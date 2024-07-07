using AutoMapper;
using DevicesApp.Dtos.Requests;
using DevicesApp.Dtos.Responses;
using DevicesApp.Models;

namespace DevicesApp.Profiles;
public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<Client, GetClientResponse>()
            .ForMember(
                    dest => dest.Name,
                    src => src.MapFrom(x => x.Name)
                    )
            .ForMember(
                    dest => dest.NationalId,
                    src => src.MapFrom(x => x.NationalId)
                    )
            .ForMember(
                     dest => dest.CreatedBy,
                     src => src.MapFrom(x => x.CreatedBy)
                    )
            .ForMember(
                    dest => dest.CreatedAt,
                    src => src.MapFrom(x => x.CreatedAt)
                    )
            .ForMember(
                    dest => dest.UpdatedAt,
                    src => src.MapFrom(x => x.UpdatedAt)
                    );

        CreateMap<CreateClientRequest, Client>()
            .ForMember(
                dest => dest.Name,
                src => src.MapFrom(x => x.Name)
                )
            .ForMember(
                dest => dest.NationalId,
                src => src.MapFrom(x => x.NationalId)
                )
            .ForMember(
                dest => dest.Latitude,
                src => src.MapFrom(x => x.Latitude)
                )
            .ForMember(
                dest => dest.Longitude,
                src => src.MapFrom(x => x.Longitude)
                )
            .ForMember(
                dest => dest.CreatedAt,
                src => src.MapFrom(x => DateOnly.FromDateTime(DateTime.UtcNow))
                );

        CreateMap<UpdateClientRequest, Client>()
            .ForMember(
                dest => dest.Name,
                src => src.MapFrom(x => x.Name)
                )
            .ForMember(
                dest => dest.NationalId,
                src => src.MapFrom(x => x.NationalId)
                )
            .ForMember(
                dest => dest.Latitude,
                src => src.MapFrom(x => x.Latitude)
                )
            .ForMember(
                dest => dest.Longitude,
                src => src.MapFrom(x => x.Longitude)
                );
    }
}
