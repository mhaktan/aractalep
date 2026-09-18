using System;
using System.Collections.Generic;
using aractalep.VehicleRequests.Dto;

namespace aractalep.VehicleRequestTypes.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class VehicleRequestTypeReportDto
    {
        public VehicleRequestTypeDto Data { get; set; }
        public List<VehicleRequestDto> VehicleRequests { get; set; }
    }
}
