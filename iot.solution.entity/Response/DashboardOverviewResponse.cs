using System;
using System.Collections.Generic;

namespace iot.solution.entity
{

    public class DashboardOverviewResponse
    {
        public Guid Guid { get; set; }
        public string TotalBuilding { get; set; }
        public string TotalDisconnectedElevators { get; set; }
        public string TotalConnectedElevators { get; set; }
        public string TotalScheduledCount { get; set; }
        public string TotalUnderMaintenanceCount { get; set; }
        public string TotalAlertCount { get; set; }
        public string TotalEnergyCount { get; set; }
        public string MinElevatorUniqueId { get; set; }
        public string MinElevatorEnergyCount { get; set; }
        public string MaxElevatorUniqueId { get; set; }
        public string MaxElevatorEnergyCount { get; set; }
        public string TotalElevators { get; set; }

        public string CriticalAlertCount { get; set; }

        public string InformationAlertCount { get; set; }

        public string MajorAlertCount { get; set; }

        public string MinorAlertCount { get; set; }
        public string WarningAlertCount { get; set; }
        public string TotalAlert { get; set; }
        public int ActiveUserCount { get; set; }
        public int InactiveUserCount { get; set; }
        public int TotalUserCount { get; set; }
        public string MinElevatorName { get; set; }
        public string MaxElevatorName { get; set; }  
        public Dictionary<string, string> Alerts { get; set; }

    }



    
   


}
