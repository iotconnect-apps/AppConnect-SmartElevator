using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IDeviceService
    {
        List<Entity.Elevator> Get();
        Entity.Elevator Get(Guid id);
        Entity.ActionStatus Manage(Entity.ElevatorModel elevator);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.Elevator>> List(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.DeviceDetailResponse>> GetEntityDeviceDetailList(Entity.SearchRequest request);
        Entity.BaseResponse<Response.DeviceDetailsResponse> GetDeviceDetail(Guid deviceId, DateTime? currentDate = null, string timeZone = "");
        List<Entity.Elevator> GetEntityDeviceList(Guid entityId);
        Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Entity.ActionStatus AcquireDevice(string uniqueid);
        Entity.SearchResult<List<Entity.Elevator>> ChildDeviceList(Entity.SearchRequest request);
        List<Response.EntityDevicesResponse> GetEntityDevices(Guid entityId);
        List<Response.EntityDevicesResponse> GetEntityChildDevices(Guid deviceId);
        Entity.BaseResponse<int> ValidateKit(string kitCode);
        Entity.BaseResponse<bool> ProvisionKit(Entity.ProvisionKitRequest request);
        Entity.BaseResponse<Entity.DeviceCounterResult> GetDeviceCounters();
        Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>> GetTelemetryData(Guid deviceId);
        Entity.BaseResponse<Entity.DeviceCounterByEntityResult> GetDeviceCountersByEntity(Guid entityGuid);
        Entity.BaseResponse<Entity.DeviceConnectionStatusResult> GetConnectionStatus(string uniqueId);
    }
}
