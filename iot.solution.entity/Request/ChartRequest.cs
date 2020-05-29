using System;
using System.Collections.Generic;

namespace iot.solution.entity.Request
{
    public class ChartRequest
    {
        public Guid BuildingId { get; set; }
        public string Frequency { get; set; }
    }
    public class ElevatorsPeakRequest
    {
        public List<string> ElevatorList { get; set; }
        public string Frequency { get; set; }
    }
}

