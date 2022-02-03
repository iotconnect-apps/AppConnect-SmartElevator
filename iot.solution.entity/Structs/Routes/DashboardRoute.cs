using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class DashboardRoute
    {
        public struct Name
        {
            public const string GetBuildings = "dashboard.buildings";
            public const string GetBuildingOverView = "dashboard.buildingoverview";
            public const string GetOperatingGraph = "dashboard.operatinggraph";
            public const string GetOverview = "dashboard.getoverview";
            public const string GetEntityDetail = "dashboard.getentitydetail";
            public const string GetDeviceDetail = "dashboard.getdevicedetail";
            public const string GetEntityCorp = "dashboard.getentitycorp";
            public const string GetEntityDevices = "dashboard.getentitydevices";
            public const string GetEntityChildDevices = "dashboard.getentitychilddevices";
            public const string GetDeviceTripDetail = "dashboard.getdevicetripdetail";
            public const string GetBuidingDetailOverview = "dashboard.getbuidingdetailoverview";
            public const string GetDevicesPeakHours = "dashboard.getdevicespeakhours";

            public const string GetMasterWidget = "configuration.getmasterwidget";
            public const string GetMasterWidgetById = "configuration.getmasterwidgetbyid";
            public const string Manage = "configuration.managemasterwidget";
            public const string DeleteMasterWidget = "configuration.deletemasterwidget";
            public const string GetUserWidget = "configuration.getuserwidget";
            public const string GetUserWidgetById = "configuration.getuserwidgetbyid";
            public const string ManageUserWidget = "configuration.manageuserwidget";
            public const string DeleteUserWidget = "configuration.deleteuserwidget";
        }
        public struct Route
        {
            public const string Global = "api/dashboard";
            public const string GetBuildings = "buildings";
            public const string GetBuildingOverView = "buildingoverview/{buildingId}";
            public const string GetOperatingGraph = "operatinggraph";
            public const string GetOverview = "overview";
            public const string GetEntityDetail = "getentitydetail/{entityId}";
            public const string GetDeviceDetail = "getdevicedetail/{deviceId}";
            public const string GetEntityCorp = "getentitycorp/{entityId}";
            public const string GetEntityDevices = "getentitydevices/{entityId}";
            public const string GetEntityChildDevices = "getentitychilddevices/{deviceId}";
            public const string GetBuidingDetailOverview = "getbuidingdetailoverview/{entityId}";
            public const string GetDeviceTripDetail = "getdevicetripdetail/{elevatorId}";
            public const string GetDevicesPeakHours = "getdevicespeakhours";

            public const string GetMasterWidget = "getmasterwidget";
            public const string GetMasterWidgetById = "getmasterwidgetbyid/{widgetId}";
            public const string Manage = "managemasterwidget";
            public const string DeleteMasterWidget = "deletemasterwidget/{id}";

            public const string GetUserWidget = "getuserwidget";
            public const string GetUserWidgetById = "getuserwidgetbyid/{widgetId}";
            public const string ManageUserWidget = "manageuserwidget";
            public const string DeleteUserWidget = "deleteuserwidget/{id}";
        }
    }
}
