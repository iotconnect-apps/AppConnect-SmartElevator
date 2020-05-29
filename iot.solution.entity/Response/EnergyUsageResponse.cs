using System;
using System.Collections.Generic;

namespace iot.solution.entity.Response
{
    public class EnergyUsageResponse
    {
        public string Month { get; set; }
        public string Value { get; set; }
    }
    public class OperationHours
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string Attribute { get; set; }
        public string OperatingHours { get; set; }
        public string EnergyConsumption { get; set; }
        public string Value { get; set; }
        public string ElevatorName { get; set; }
    }
    public class Energy
    {
        public string Day { get; set; }
        public string UniqueId { get; set; }
        public string Attribute { get; set; }
        public string OperatingHours { get; set; }
    }
    //TripsByElevator
    public class TripsByElevator
    {
        public string Day { get; set; }
        public string Attribute { get; set; }
        public string UniqueId { get; set; }
        public string EnergyConsumption { get; set; }
    }

    public class ConfgurationResponse
    {
        public string cpId { get; set; }
        public string host { get; set; }
        public int isSecure { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public string url { get; set; }
        public string user { get; set; }
        public string vhost { get; set; }
    }

    
}
