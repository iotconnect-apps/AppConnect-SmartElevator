using iot.solution.entity.Structs.Routes;
using iot.solution.host.Filter;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(DeviceRoute.Route.Global)]
    [ApiController]
    public class DeviceController : BaseController
    {
        private readonly IDeviceService _service;
        private readonly IEntityService _entityService;
        public DeviceController(IDeviceService deviceEngine, IEntityService entityService)
        {
            _service = deviceEngine;
            _entityService = entityService;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.GetList, Name = DeviceRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.Elevator>> Get()
        {
            Entity.BaseResponse<List<Entity.Elevator>> response = new Entity.BaseResponse<List<Entity.Elevator>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.Elevator>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.GetById, Name = DeviceRoute.Name.GetById)]
        [EnsureGuidParameterAttribute("id", "Device")]
        public Entity.BaseResponse<Entity.Elevator> Get(string id)
        {
            if (id == null || Guid.Parse(id) == Guid.Empty)
            {
                return new Entity.BaseResponse<Entity.Elevator>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.Elevator> response = new Entity.BaseResponse<Entity.Elevator>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Elevator>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceRoute.Route.Manage, Name = DeviceRoute.Name.Add)]
        public Entity.BaseResponse<Guid> Manage([FromForm]Entity.ElevatorModel request)
        {
            Entity.BaseResponse<Guid> response = new Entity.BaseResponse<Guid>(true);
            try
            {
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Guid>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(DeviceRoute.Route.Delete, Name = DeviceRoute.Name.Delete)]
        [EnsureGuidParameterAttribute("id", "Device")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            if (id == null || Guid.Parse(id) == Guid.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Invalid Request");
            }

            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.BySearch, Name = DeviceRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.Elevator>>> GetBySearch(string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.Elevator>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.Elevator>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.Elevator>>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceRoute.Route.UpdateStatus, Name = DeviceRoute.Name.UpdateStatus)]
        [EnsureGuidParameterAttribute("id", "Device")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceRoute.Route.AcquireDevice, Name = DeviceRoute.Name.AcquireDevice)]
        public Entity.BaseResponse<bool> AcquireDevice(string uniqueId, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.AcquireDevice(uniqueId);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.GetEntityDevicesDetails, Name = DeviceRoute.Name.GetEntityDevicesDetails)]
        [EnsureGuidParameterAttribute("entityId", "Device")]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>> GetEntityDevicesDetails(string entityId,string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>>(true);
            try
            {
                response.Data = _service.GetEntityDeviceDetailList(new Entity.SearchRequest()
                {
                    EntityId = Guid.Parse(entityId),
                    SearchText = searchText,
                    PageNumber = -1,//pageNo.Value,
                    PageSize =-1,// pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.ValidateKit, Name = DeviceRoute.Name.ValidateKit)]
        public Entity.BaseResponse<int> ValidateKit(string kitCode)
        {
            Entity.BaseResponse<int> response = new Entity.BaseResponse<int>(true);
            try
            {
                response = _service.ValidateKit(kitCode);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<int>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceRoute.Route.ProvisionKit, Name = DeviceRoute.Name.ProvisionKit)]
        public Entity.BaseResponse<bool> ProvisionKit([FromForm]Entity.ProvisionKitRequest request)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                response = _service.ProvisionKit(request);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.DeviceCounters, Name = DeviceRoute.Name.DeviceCounters)]
        public Entity.BaseResponse<Entity.DeviceCounterResult> DeviceCounters()
        {
            try
            {
                return _service.GetDeviceCounters();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.DeviceCounterResult>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route(DeviceRoute.Route.TelemetryData, Name = DeviceRoute.Name.TelemetryData)]
        [EnsureGuidParameterAttribute("deviceId", "Device")]
        public Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>> GetTelemetryData(string deviceId)
        {
            try
            {
                return _service.GetTelemetryData(Guid.Parse(deviceId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route(DeviceRoute.Route.ConnectionStatus, Name = DeviceRoute.Name.ConnectionStatus)]
        public Entity.BaseResponse<Entity.DeviceConnectionStatusResult> ConnectionStatus(string uniqueId)
        {
            try
            {
                return _service.GetConnectionStatus(uniqueId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.DeviceConnectionStatusResult>(false, ex.Message);
            }
        }
    }
}