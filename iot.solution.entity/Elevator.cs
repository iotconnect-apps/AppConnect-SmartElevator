using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace iot.solution.entity
{
    public class Elevator
    {
        public Guid? Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        [Required]
        public Guid EntityGuid { get; set; }
        public Guid TemplateGuid { get; set; }
        public Guid TypeGuid { get; set; }
        [Required]
        public string UniqueId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Building { get; set; }
        public string Wing { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public string Note { get; set; }
        public string Tag { get; set; }
        public string Image { get; set; }
        public string KitCode { get; set; }
        public bool IsProvisioned { get; set; }
        public bool isedit { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
    public class ElevatorModel : Elevator
    {
        public IFormFile ImageFile { get; set; }

    }
   
}
