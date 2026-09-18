using System;
using System.Collections.Generic;
using aractalep.VehicleRequests.Dto;

namespace aractalep.Vehicles.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class VehicleReportDto
    {
        public VehicleDto Data { get; set; }
        public List<VehicleRequestDto> VehicleRequests { get; set; }
    }
}
