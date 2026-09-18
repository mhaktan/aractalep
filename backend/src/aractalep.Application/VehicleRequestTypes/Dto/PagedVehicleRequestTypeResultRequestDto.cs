using System;
using Abp.Application.Services.Dto;

namespace aractalep.VehicleRequestTypes.Dto
{
    public class PagedVehicleRequestTypeResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
