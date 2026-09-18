using AutoMapper;
using aractalep.Entities;
using aractalep.Vehicles.Dto;

namespace aractalep.Vehicles
{
    public class VehicleMapProfile : Profile
    {
        public VehicleMapProfile()
        {
            CreateMap<Vehicle, VehicleDto>();
            CreateMap<CreateVehicleDto, Vehicle>();
            CreateMap<VehicleDto, Vehicle>();
        }
    }
}
