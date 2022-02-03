using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class ChartRoute
    {
        public struct Name
        {
            public const string GetPeakHoursByElevator = "chart.getpeakhoursbyelevator";
            public const string GetOperationHours = "chart.getoperationhours";
            public const string TripsByElevator = "chart.tripsbyelevator";
            public const string ExecuteCrone = "chart.executecrone";
        }

        public struct Route
        {
            public const string Global = "api/chart";
            public const string GetPeakHoursByElevator = "getpeakhoursbyelevator";
            public const string GetOperationHours = "getoperationhours";
            public const string TripsByElevator = "gettripsbyelevator/{elevatorId}";
            public const string ExecuteCrone = "executecron";
        }
    }
}
