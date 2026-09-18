using System;
using System.Collections.Generic;
using aractalep.VehicleRequests.Dto;

namespace aractalep.Departments.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class DepartmentReportDto
    {
        public DepartmentDto Data { get; set; }
        public List<VehicleRequestDto> VehicleRequests { get; set; }
    }
}
