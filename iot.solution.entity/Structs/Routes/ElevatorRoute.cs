using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class ElevatorRoute
    {
        public struct Name
        {
            public const string Add = "elevator.add";
            public const string GetList = "elevator.list";
            public const string GetById = "elevator.getbyid";
            public const string Delete = "elevator.delete";
            public const string DeleteMediaFile = "elevator.deletemediafile";
            public const string BySearch = "elevator.search";
            public const string UpdateStatus = "elevator.updatestatus";
            public const string EnergyUsage = "elevator.energyusage";
            public const string WaterUsage = "elevator.waterusage";
            public const string SoilNutrition = "elevator.soilnutrition";
            public const string FileUpload = "elevator.fileupload";
            public const string ValidateKit = "elevator.validatekit";
            public const string ProvisionKit = "elevator.provisionkit";
        }

        public struct Route
        {
            public const string Global = "api/elevator";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete/{id}";
            public const string DeleteMediaFile = "deletemediafile/{elevatorId}/{fileId?}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
            public const string BySearch = "search";
            public const string EnergyUsage = "getenergyusage/{elevatorId}";
            public const string WaterUsage = "getwaterusage/{elevatorId}";
            public const string SoilNutrition = "getsoilnutrition/{elevatorId}";
            public const string FileUpload = "fileupload/{elevatorId}";
            public const string ValidateKit = "validatekit/{kitCode}";
            public const string ProvisionKit = "provisionkit";
        }
    }
}
