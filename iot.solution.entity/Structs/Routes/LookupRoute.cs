namespace iot.solution.entity.Structs.Routes
{
    public struct LookupRoute
    {
        public struct Name
        {
            public const string Get = "lookup.get";
            public const string GetTemplate = "lookup.template";
            public const string GetAllTemplate = "lookup.alltemplate";
            public const string GetTagLookup = "lookup.attributes";
            public const string GetTemplateCommands = "lookup.GetTemplateCommands";
            public const string GetBuildingLookup = "lookup.getbuildinglookup";
            public const string GetWingLookup = "lookup.getwinglookup";
            public const string GetElevatorLookup = "lookup.getelevatorlookup";            
            public const string GetAllTemplateIoT = "lookup.alltemplateiot";
            public const string GetAttributesIoT = "lookup.allattributesiot";
            public const string GetSensorsLookup = "lookup.sensors";            
            public const string GetCommandsIoT = "allcommandsiot/{templateGuid}";
            public const string GetElevatorLookupByBuilding = "lookup.getelevatorlookupbybuilding";
            public const string GetElevatorLookupByCompany = "lookup.getelevatorlookupbycompany";
        }

        public struct Route
        {
            public const string Global = "api/lookup";
            public const string Get = "{type}/{param?}";
            public const string GetTemplate = "template";
            public const string GetAllTemplate = "alltemplate";
            public const string GetTagLookup = "attributes/{templateId}";
            public const string GetTemplateCommands = "commands/{templateId}";
            public const string GetBuildingLookup = "buildinglookup/{companyId}";
            public const string GetWingLookup= "winglookup/{buildingId}";
            public const string GetElevatorLookup= "elevatorlookup/{wingId}";            
            public const string GetAllTemplateIoT = "alltemplateiot";
            public const string GetAttributesIoT = "allattributesiot/{templateGuid}";
            public const string GetCommandsIoT = "allcommandsiot/{templateGuid}";
            public const string GetSensorsLookup = "sensors/{templateId}/{deviceId}";
            public const string GetElevatorLookupByBuilding = "getelevatorlookupbybuilding/{buildingId}";
            public const string GetElevatorLookupByCompany = "getelevatorlookupbycompany";
        }
    }
}
