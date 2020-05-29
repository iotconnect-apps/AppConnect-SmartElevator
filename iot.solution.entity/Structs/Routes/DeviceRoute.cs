using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct DeviceRoute
    {
        public struct Name
        {
            public const string Add = "device.add";
            public const string GetList = "device.list";
            public const string GetById = "device.getdevicebyid";
            public const string Delete = "device.deletedevice";
            public const string BySearch = "device.search";
            public const string UpdateStatus = "device.updatestatus";
            public const string AcquireDevice = "device.acquire";
            public const string ChildDevice = "device.childdevicelist";
            public const string ValidateKit = "device.validatekit";
            public const string ProvisionKit = "device.provisionkit";
            public const string GetEntityDevices = "device.getentitydevices";
            public const string GetEntityDevicesDetails = "device.getentitydevicesdetails";
            public const string DeviceCounters = "device.counters";
            public const string TelemetryData = "device.telemetry";
            public const string ConnectionStatus = "device.connectionstatus";
        }

        public struct Route
        {
            public const string Global = "api/device";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete/{id}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
            public const string AcquireDevice = "acquire/{uniqueId}";
            public const string BySearch = "search";
            public const string ChildDevice = "childdevicelist";
            public const string ValidateKit = "validatekit/{kitCode}";
            public const string ProvisionKit = "provisionkit";
            public const string GetEntityDevices = "getentitydevices/{entityId}";
            public const string GetEntityDevicesDetails = "getentitydevicesdetails/{entityId}";
            public const string DeviceCounters = "counters";
            public const string TelemetryData = "telemetry/{deviceId}";
            public const string ConnectionStatus = "connectionstatus/{uniqueId}";
        }
    }
}
