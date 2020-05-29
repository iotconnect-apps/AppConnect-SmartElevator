using System;
using System.Collections.Generic;

namespace iot.solution.host.Models
{
    public partial class ElevatorMaintenance
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid ElevatorGuid { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
