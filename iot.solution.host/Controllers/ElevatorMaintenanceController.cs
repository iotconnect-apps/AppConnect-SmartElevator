using iot.solution.entity;
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
    [Route(ElevatorMaintenanceRoute.Route.Global)]
    [ApiController]
    public class ElevatorMaintenanceController : BaseController
    {

        private readonly IElevatorMaintenanceService _maintenanceService;
        public ElevatorMaintenanceController(IElevatorMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        [Route(ElevatorMaintenanceRoute.Route.GetList, Name = ElevatorMaintenanceRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.ElevatorMaintenance>> Get()
        {
            Entity.BaseResponse<List<Entity.ElevatorMaintenance>> response = new Entity.BaseResponse<List<Entity.ElevatorMaintenance>>(true);
            try
            {
                response.Data = _maintenanceService.Get();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.ElevatorMaintenance>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(ElevatorMaintenanceRoute.Route.GetById, Name = ElevatorMaintenanceRoute.Name.GetById)]
        [EnsureGuidParameterAttribute("id", "Elevator Maintenance")]
        public Entity.BaseResponse<Entity.ElevatorMaintenance> Get(string id, DateTime currentDate, string timeZone)
        {
            if (id == null || Guid.Parse(id) == Guid.Empty)
            {
                return new Entity.BaseResponse<Entity.ElevatorMaintenance>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.ElevatorMaintenance> response = new Entity.BaseResponse<Entity.ElevatorMaintenance>(true);
            try
            {

                response.Data = _maintenanceService.Get(Guid.Parse(id),currentDate, timeZone);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.ElevatorMaintenance>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(ElevatorMaintenanceRoute.Route.Manage, Name = ElevatorMaintenanceRoute.Name.Add)]
        public Entity.BaseResponse<Entity.ElevatorMaintenance> Manage([FromBody]Entity.ElevatorMaintenance request)
        {

            Entity.BaseResponse<Entity.ElevatorMaintenance> response = new Entity.BaseResponse<Entity.ElevatorMaintenance>(true);
            try
            {
                var status = _maintenanceService.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.ElevatorMaintenance>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(ElevatorMaintenanceRoute.Route.Delete, Name = ElevatorMaintenanceRoute.Name.Delete)]
        [EnsureGuidParameterAttribute("id", "Elevator Maintenance")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            if (id == null || Guid.Parse(id) == Guid.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Invalid Request");
            }

            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _maintenanceService.Delete(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(ElevatorMaintenanceRoute.Route.BySearch, Name = ElevatorMaintenanceRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>>> GetBySearch(string entityGuid = "", string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "", DateTime? currentDate = null, string timeZone = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>>>(true);
            try
            {
                response.Data = _maintenanceService.List(new Entity.SearchRequest()
                {
                    EntityId = string.IsNullOrEmpty(entityGuid) ? Guid.Empty : new Guid(entityGuid),
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy,
                    CurrentDate = currentDate,
                    TimeZone = timeZone
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(ElevatorMaintenanceRoute.Route.UpdateStatus, Name = ElevatorMaintenanceRoute.Name.UpdateStatus)]
        [EnsureGuidParameterAttribute("id", "Elevator Maintenance")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, string status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {

                Entity.ActionStatus result = _maintenanceService.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;

            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(ElevatorMaintenanceRoute.Route.UpComingList, Name = ElevatorMaintenanceRoute.Name.UpComingList)]
        public Entity.BaseResponse<List<Entity.ElevatorMaintenanceResponse>> UpcomingList(string buildingid = "", string elevatorid = "", DateTime? currentDate = null, string timeZone = "")
        {
            Entity.BaseResponse<List<Entity.ElevatorMaintenanceResponse>> response = new Entity.BaseResponse<List<Entity.ElevatorMaintenanceResponse>>(true);
            try
            {
                //response.Data = new List<ElevatorMaintenanceResponse>(); ;
                //response.Data.Add(new ElevatorMaintenanceResponse() { BuildingName = "Building 1", ElevatorName = "Elevator 1", Description = "Maintence of Elevator need to be done on mentioned date.", ScheduledDate = DateTime.Now.AddDays(5) });
                //response.Data.Add(new ElevatorMaintenanceResponse() { BuildingName = "Building 1", ElevatorName = "Elevator 2", Description = "Maintence of Elevator need to be done on mentioned date.", ScheduledDate = DateTime.Now.AddDays(20) });
                //response.Data.Add(new ElevatorMaintenanceResponse() { BuildingName = "Building 2", ElevatorName = "Elevator 3", Description = "Maintence of Elevator need to be done on mentioned date.", ScheduledDate = DateTime.Now.AddDays(10) });
                //response.Data.Add(new ElevatorMaintenanceResponse() { BuildingName = "Building 3", ElevatorName = "Elevator 4", Description = "Maintence of Elevator need to be done on mentioned date.", ScheduledDate = DateTime.Now.AddDays(8) });
                //response.Data.Add(new ElevatorMaintenanceResponse() { BuildingName = "Building 1", ElevatorName = "Elevator 5", Description = "Maintence of Elevator need to be done on mentioned date.", ScheduledDate = DateTime.Now.AddDays(9) });
                //response.Data.Add(new ElevatorMaintenanceResponse() { BuildingName = "Building 2", ElevatorName = "Elevator 6", Description = "Maintence of Elevator need to be done on mentioned date.", ScheduledDate = DateTime.Now.AddDays(6) });
                //response.Data.Add(new ElevatorMaintenanceResponse() { BuildingName = "Building 3", ElevatorName = "Elevator 7", Description = "Maintence of Elevator need to be done on mentioned date.", ScheduledDate = DateTime.Now.AddDays(4) });
                var request = new ElevatorMaintenanceRequest();
                if (!string.IsNullOrWhiteSpace(buildingid)) {
                    request.BuildingGuid = Guid.Parse(buildingid);
                }
                if (!string.IsNullOrWhiteSpace(elevatorid))
                {
                    request.ElevatorGuid = Guid.Parse(elevatorid);
                }
               
                request.CurrentDate = currentDate;
                request.TimeZone = timeZone;
                
                response.Data = _maintenanceService.GetUpComingList(request);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.ElevatorMaintenanceResponse>>(false, ex.Message);
            }
            return response;
        }

    }
}