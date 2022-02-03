using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace iot.solution.entity
{
    public class ElevatorMaintenance
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        //This is wing Guid
        [Required]
        public Guid? EntityGuid { get; set; }
        [Required]
        public Guid? BuildingGuid { get; set; }
        [Required]
        public Guid ElevatorGuid { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? StartDateTime { get; set; }
        [Required]
        public DateTime? EndDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public string TimeZone { get; set; }
    }
    public class ElevatorMaintenanceDetail : ElevatorMaintenance
    {
        public string Name { get; set; }
        public string Building { get; set; }
        public string Wing { get; set; }

    }

    public class ElevatorMaintenanceResponse: ElevatorMaintenance
    {
        public string Building { get; set; }
        public string Wing{ get; set; }
        public string ElevatorName { get; set; }
    }

    public class ElevatorMaintenanceRequest
    {
        public Guid? BuildingGuid { get; set; }
        public Guid? ElevatorGuid { get; set; }
        public DateTime? CurrentDate { get; set; }
        public string TimeZone { get; set; }
    }
}
