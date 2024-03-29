﻿using iot.solution.entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface ILookupService
    {
        List<Entity.LookupItem> Get(string type, string param);
        List<Entity.LookupItem> GetTemplate(bool isGateway);
        List<Entity.LookupItem> GetAllTemplate();
        List<Entity.TagLookup> GetTagLookup(Guid templateId);
        List<Entity.LookupItem> GetTemplateAttribute(Guid templateId);
        List<Entity.LookupItem> GetTemplateCommands(Guid templateId);

        List<Entity.LookupItemWithStatus> BuildingLookup(Guid templateId);

        List<Entity.LookupItem> WingLookup(Guid templateId);

        List<Entity.LookupItemWithStatus> ElevatorLookup(Guid templateId);
        Entity.SearchResult<List<Entity.ElevatorLookupDetail>> ElevatorLookupByCompany();
        string GetIotTemplateGuidByCode();
        List<Entity.LookupItem> GetSensors(Guid templateId, Guid deviceId);
        List<Entity.LookupItem> GetAllTemplateFromIoT();
        List<Entity.KitTypeAttribute> GetAllAttributesFromIoT(string templateGuid);
        List<Entity.LookupItem> GetAllCommandsFromIoT(string templateGuid);
        Entity.BaseResponse<List<Entity.BuildingElevatorLookup>> ElevatorLookupByBuilding(Guid templateId);
    }
}