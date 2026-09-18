using AutoMapper;
using aractalep.Entities;
using aractalep.VehicleRequests.Dto;

namespace aractalep.VehicleRequests
{
    public class VehicleRequestMapProfile : Profile
    {
        public VehicleRequestMapProfile()
        {
            CreateMap<VehicleRequest, VehicleRequestDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateVehicleRequestDto, VehicleRequest>();
            CreateMap<VehicleRequestDto, VehicleRequest>();
        }
    }
}
