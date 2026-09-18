using AutoMapper;
using aractalep.Entities;
using aractalep.VehicleRequestTypes.Dto;

namespace aractalep.VehicleRequestTypes
{
    public class VehicleRequestTypeMapProfile : Profile
    {
        public VehicleRequestTypeMapProfile()
        {
            CreateMap<VehicleRequestType, VehicleRequestTypeDto>();
            CreateMap<CreateVehicleRequestTypeDto, VehicleRequestType>();
            CreateMap<VehicleRequestTypeDto, VehicleRequestType>();
        }
    }
}
