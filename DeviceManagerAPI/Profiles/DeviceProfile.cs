using AutoMapper;
using DevicesApp.Dtos.Requests;
using DevicesApp.Dtos.Responses;
using DevicesApp.Models;

namespace DevicesApp.Profiles;
public class DeviceProfile : Profile
{
    public DeviceProfile()
    {
        CreateMap<Device, GetDeviceResponse>()
            .ForMember(
                    dest => dest.SerialNo,
                    src => src.MapFrom(x => x.SerialNo)
                    )
            .ForMember(
                    dest => dest.Name,
                    src => src.MapFrom(x => x.Name)
                    )
            .ForMember(
                    dest => dest.Status,
                    src => src.MapFrom(x => x.Status)
                    )
            .ForMember(
                     dest => dest.CreatedAt,
                     src => src.MapFrom(x => x.CreatedAt)
                    )
            .ForMember(
                     dest => dest.UpdatedAt,
                     src => src.MapFrom(x => x.UpdatedAt)
                    )
            .ForMember(
                    dest => dest.CreatedBy,
                    src => src.MapFrom(x => x.CreatedBy)
                    )
            .ForMember(
                    dest => dest.TypeId,
                    src => src.MapFrom(x => x.TypeId)
                    );

        CreateMap<CreateDeviceRequest, Device>()
            .ForMember(
                dest => dest.SerialNo,
                src => src.MapFrom(x => x.SerialNo)
                )
            .ForMember(
                dest => dest.Name,
                src => src.MapFrom(x => x.Name)
                )
            .ForMember(
                dest => dest.Status,
                src => src.MapFrom(x => DeviceStatus.OFF)
                )
            .ForMember(
                dest => dest.CreatedAt,
                src => src.MapFrom(x => DateOnly.FromDateTime(DateTime.UtcNow))
                )
            .ForMember(
                dest => dest.UpdatedAt,
                src => src.MapFrom(x => DateOnly.FromDateTime(DateTime.UtcNow))
                )
            .ForMember(
                dest => dest.TypeId,
                src => src.MapFrom(x => x.TypeId)
                );

        CreateMap<UpdateDeviceRequest, Device>()
            .ForMember(
                dest => dest.SerialNo,
                src => src.MapFrom(x => x.SerialNo)
                )
            .ForMember(
                dest => dest.Name,
                src => src.MapFrom(x => x.Name)
                )
            .ForMember(
                dest => dest.TypeId,
                src => src.MapFrom(x => x.TypeId)
                );
    }
}
