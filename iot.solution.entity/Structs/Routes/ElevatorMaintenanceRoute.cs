using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class ElevatorMaintenanceRoute
    {
        public struct Name
        {
            public const string Add = "elevatormaintenance.add";
            public const string GetList = "elevatormaintenance.list";
            public const string GetStatusList = "elevatormaintenance.statuslist";
            public const string GetById = "elevatormaintenance.getbyid";
            public const string Delete = "elevatormaintenance.delete";
            public const string BySearch = "elevatormaintenance.search";
            public const string UpdateStatus = "elevatormaintenance.updatestatus";
            public const string UpComingList = "elevatormaintenance.upcoming";

        }

        public struct Route
        {
            public const string Global = "api/elevatormaintenance";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string UpComingList = "upcoming";
            public const string GetStatusList = "status";
            public const string GetById = "{id}";
            public const string Delete = "delete/{id}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
            public const string BySearch = "search";            
        }
    }
}
