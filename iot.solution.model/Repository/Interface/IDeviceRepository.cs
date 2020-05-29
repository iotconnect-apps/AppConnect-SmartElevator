using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.model.Repository.Interface
{
    public interface IDeviceRepository : IGenericRepository<Model.Elevator>
    {
        Model.Elevator Get(string device);
        Entity.ActionStatus Manage(Model.Elevator request,bool IsEdit=false);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.Elevator>> List(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.DeviceDetailResponse>> DetailList(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request);
        Entity.SearchResult<List<Model.Elevator>> GetChildDevice(Entity.SearchRequest request);
        List<Entity.LookupItem> GetGetwayLookup();
        List<Entity.LookupItem> GetDeviceLookup();
        List<Response.EntityDevicesResponse> GetEntityDevices(Guid? entityId, Guid? deviceId);
        Entity.BaseResponse<int> ValidateKit(string kitCode);
        Entity.BaseResponse<List<Entity.HardwareKit>> ProvisionKit(Entity.ProvisionKitRequest request);
        Entity.BaseResponse<List<Entity.BuildingElevatorLookup>> GetDeviceLookupByEntityId(Guid buildingId);
        Entity.BaseResponse<Response.DeviceDetailsResponse> GetDeviceDetail(Guid deviceId);
    }
}
