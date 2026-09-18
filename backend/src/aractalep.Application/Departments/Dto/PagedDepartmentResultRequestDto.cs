using System;
using Abp.Application.Services.Dto;

namespace aractalep.Departments.Dto
{
    public class PagedDepartmentResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
    }
}
