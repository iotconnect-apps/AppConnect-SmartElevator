using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class ElevatorMaintenance
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid ElevatorGuid { get; set; }
        public string Description { get; set; }
        //public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
