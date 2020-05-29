using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace iot.solution.entity
{
    public class ElevatorMaintenance
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        //This is wing Guid
        public Guid? EntityGuid { get; set; }
        public Guid? BuildingGuid { get; set; }
        public Guid ElevatorGuid { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }       
        public DateTime? CreatedDate { get; set; }
        public DateTime ScheduledDate { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class ElevatorMaintenanceDetail : ElevatorMaintenance
    {
        public string Name { get; set; }
        public string Building { get; set; }
        public string Wing { get; set; }

    }

    public class ElevatorMaintenanceResponse
    {
        public string Building { get; set; }
        public string Wing{ get; set; }
        public string ElevatorName { get; set; }
        public string Description { get; set; }
        public DateTime ScheduledDate { get; set; }
    }

    public class ElevatorMaintenanceRequest
    {
        public Guid? BuildingGuid { get; set; }
        public Guid? ElevatorGuid { get; set; }
    }
}
