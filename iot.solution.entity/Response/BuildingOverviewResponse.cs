namespace iot.solution.entity
{
    public class BuildingOverviewResponse
    {
        public string TotalElevator { get; set; }

        public string TotalOperatingHours { get; set; }

        public string TotalTrips { get; set; }

        public string TotalEnergy { get; set; }

        public string TotalConnectedElevator { get; set; }

        public string TotalMaintenance { get; set; }
        public string TotalAlerts { get; set; }

    }

    public class OperatingGraphResponse
    {
        public string Label { get; set; }
        public float OperatingHours { get; set; }
        public int EnergyConsumption { get; set; }

    }



}
