using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace aractalep.Departments.Dto
{
    [AutoMapTo(typeof(Entities.Department))]
    public class CreateDepartmentDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

    }
}