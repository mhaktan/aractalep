using System;
using Abp.Application.Services.Dto;

namespace aractalep.Vehicles.Dto
{
    public class PagedVehicleResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Plate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public int? Capacity { get; set; }
        public int? Status { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? CapacityFrom { get; set; }
        public int? CapacityTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
