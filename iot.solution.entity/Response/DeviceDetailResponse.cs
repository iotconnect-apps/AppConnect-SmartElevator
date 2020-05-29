using System.Collections.Generic;

namespace iot.solution.entity.Response
{
    public class DeviceDetailsResponse
    {
        public string UniqueId { get; set; }
        public int OperatingHourCount { get; set; }
        public int AverageTrip { get; set; }
        public int EnergyCount { get; set; }
        public int AverageTemperature { get; set; }
        public int AverageSpeed { get; set; }
        public int AverageVibration { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }

   
    public class DeviceTripsResponse
    {
        public string Time { get; set; }
        public int Value { get; set; }

    }
    public class DevicePeakHoursResponse
    {
        public string Time { get; set; }
        public List<PeakLookup> Value { get; set; }

    }
    public class PeakLookup
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class BuidingDetailOverviewResponse
    {
        public int AverageOperatingHours { get; set; }
        public int AverageTrips { get; set; }
        public int Energy { get; set; }
        public int TotalMaintenanceCarriedOut { get; set; }
        public int Alerts { get; set; }
        public ElevatorsCount ElevatorsCounts { get; set; }
    }

    public class ElevatorsCount
    {
        public int Connected { get; set; }
        public int Total { get; set; }
    }


}
