using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Request = iot.solution.entity.Request;
using iot.solution.host.Filter;

namespace host.iot.solution.Controllers
{
    [Route(DashboardRoute.Route.Global)]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _service;
        private readonly IEntityService _entityService;
        private readonly IDeviceService _deviceService;

        public DashboardController(IDashboardService dashboardService, IEntityService entityService, IDeviceService deviceService)
        {
            _service = dashboardService;
            _entityService = entityService;
            _deviceService = deviceService;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetBuildings, Name = DashboardRoute.Name.GetBuildings)]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetBuildings()
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetBuildingsLookup();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetBuildingOverView, Name = DashboardRoute.Name.GetBuildingOverView)]
        [EnsureGuidParameterAttribute("buildingId", "Dashboard")]
        public Entity.BaseResponse<Entity.BuildingOverviewResponse> GetBuildingOverView(string buildingId, DateTime currentDate, string timeZone, string frequency = "")
        {
            Entity.BaseResponse<Entity.BuildingOverviewResponse> response = new Entity.BaseResponse<Entity.BuildingOverviewResponse>(true);
            try
            {
                response = _service.GetBuildingOverview(Guid.Parse(buildingId), frequency,currentDate,timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.BuildingOverviewResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetOperatingGraph, Name = DashboardRoute.Name.GetOperatingGraph)]

        public Entity.BaseResponse<List<Entity.OperatingGraphResponse>> GetOperatingGraph(string buildingId, string elevatorId, int type = 1)
        {
            Entity.BaseResponse<List<Entity.OperatingGraphResponse>> response = new Entity.BaseResponse<List<Entity.OperatingGraphResponse>>(true);
            try
            {
                if (type == 1)
                {
                    //type: 1 = Day, 2 = Month and 3 = Year
                    response.Data = new List<Entity.OperatingGraphResponse>();
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "Sunday", EnergyConsumption = 50, OperatingHours = 4 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "Monday", EnergyConsumption = 120, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "Tuesday", EnergyConsumption = 130, OperatingHours = 5.5F });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "Wednesday", EnergyConsumption = 90, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "Thursday", EnergyConsumption = 140, OperatingHours = 7 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "Friday", EnergyConsumption = 120, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "Saturday", EnergyConsumption = 110, OperatingHours = 5.5F });
                }
                else if (type == 2)
                {
                    //type: 1 = Day, 2 = Month and 3 = Year
                    response.Data = new List<Entity.OperatingGraphResponse>();
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "January", EnergyConsumption = 50, OperatingHours = 4 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "February", EnergyConsumption = 120, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "March", EnergyConsumption = 130, OperatingHours = 5.5F });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "April", EnergyConsumption = 90, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "May", EnergyConsumption = 140, OperatingHours = 7 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "June", EnergyConsumption = 120, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "July", EnergyConsumption = 110, OperatingHours = 5.5F });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "August", EnergyConsumption = 130, OperatingHours = 5.5F });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "September", EnergyConsumption = 90, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "October", EnergyConsumption = 140, OperatingHours = 7 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "November", EnergyConsumption = 120, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "December", EnergyConsumption = 110, OperatingHours = 5.5F });
                }
                else if (type == 3)
                {
                    //type: 1 = Day, 2 = Month and 3 = Year
                    response.Data = new List<Entity.OperatingGraphResponse>();
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "2015", EnergyConsumption = 50, OperatingHours = 4 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "2016", EnergyConsumption = 120, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "2017", EnergyConsumption = 130, OperatingHours = 5.5F });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "2018", EnergyConsumption = 90, OperatingHours = 6 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "2019", EnergyConsumption = 140, OperatingHours = 7 });
                    response.Data.Add(new Entity.OperatingGraphResponse() { Label = "2020", EnergyConsumption = 120, OperatingHours = 6 });
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.OperatingGraphResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetOverview, Name = DashboardRoute.Name.GetOverview)]
        public Entity.BaseResponse<Entity.DashboardOverviewResponse> GetOverview(DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<Entity.DashboardOverviewResponse> response = new Entity.BaseResponse<Entity.DashboardOverviewResponse>(true);
            try
            {
                response = _service.GetOverview(currentDate,timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.DashboardOverviewResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetEntityDetail, Name = DashboardRoute.Name.GetEntityDetail)]
        [EnsureGuidParameterAttribute("entityId", "Dashboard")]
        public Entity.BaseResponse<Response.EntityDetailResponse> GetEntityDetail(string entityId)
        {
            if (entityId == null || Guid.Parse(entityId) == Guid.Empty)
            {
                return new Entity.BaseResponse<Response.EntityDetailResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Response.EntityDetailResponse> response = new Entity.BaseResponse<Response.EntityDetailResponse>(true);
            try
            {
                response.Data = _entityService.GetEntityDetail(Guid.Parse(entityId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Response.EntityDetailResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DashboardRoute.Route.GetDeviceDetail, Name = DashboardRoute.Name.GetDeviceDetail)]
        [EnsureGuidParameterAttribute("deviceId", "Dashboard")]
        public Entity.BaseResponse<Response.DeviceDetailsResponse> GetDeviceDetail(string deviceId, DateTime? currentDate = null, string timeZone = "")
        {
            if (deviceId == null || Guid.Parse(deviceId) == Guid.Empty)
            {
                return new Entity.BaseResponse<Response.DeviceDetailsResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Response.DeviceDetailsResponse> response = new Entity.BaseResponse<Response.DeviceDetailsResponse>(true);
            try
            {
                response = _deviceService.GetDeviceDetail(Guid.Parse(deviceId),currentDate,timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Response.DeviceDetailsResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetEntityDevices, Name = DashboardRoute.Name.GetEntityDevices)]
        [EnsureGuidParameterAttribute("entityId", "Dashboard")]
        public Entity.BaseResponse<List<Response.EntityDevicesResponse>> GetEntityDevices(string entityId)
        {
            if (entityId == null || Guid.Parse(entityId) == Guid.Empty)
            {
                return new Entity.BaseResponse<List<Response.EntityDevicesResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.EntityDevicesResponse>> response = new Entity.BaseResponse<List<Response.EntityDevicesResponse>>(true);
            try
            {
                response.Data = _deviceService.GetEntityDevices(Guid.Parse(entityId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.EntityDevicesResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetEntityChildDevices, Name = DashboardRoute.Name.GetEntityChildDevices)]
        [EnsureGuidParameterAttribute("deviceId", "Dashboard")]
        public Entity.BaseResponse<List<Response.EntityDevicesResponse>> GetEntityChildDevices(string deviceId)
        {
            if (deviceId == null || Guid.Parse(deviceId) == Guid.Empty)
            {
                return new Entity.BaseResponse<List<Response.EntityDevicesResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.EntityDevicesResponse>> response = new Entity.BaseResponse<List<Response.EntityDevicesResponse>>(true);
            try
            {
                response.Data = _deviceService.GetEntityChildDevices(Guid.Parse(deviceId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.EntityDevicesResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetDeviceTripDetail, Name = DashboardRoute.Name.GetDeviceTripDetail)]
        [EnsureGuidParameterAttribute("elevatorId", "Dashboard")]
        public Entity.BaseResponse<List<Response.DeviceTripsResponse>> GetDeviceTripDetail(string elevatorId)
        {
            if (elevatorId == null || Guid.Parse(elevatorId) == Guid.Empty)
            {
                return new Entity.BaseResponse<List<Response.DeviceTripsResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.DeviceTripsResponse>> response = new Entity.BaseResponse<List<Response.DeviceTripsResponse>>(true);
            try
            {
                List<Response.DeviceTripsResponse> tripList = new List<Response.DeviceTripsResponse> {
                    new Response.DeviceTripsResponse { Time="0:00",Value=10},
                    new Response.DeviceTripsResponse { Time="1:00",Value=40},
                    new Response.DeviceTripsResponse { Time="2:00",Value=50},
                    new Response.DeviceTripsResponse { Time="3:00",Value=70},
                    new Response.DeviceTripsResponse { Time="4:00",Value=60},
                    new Response.DeviceTripsResponse { Time="5:00",Value=20},
                    new Response.DeviceTripsResponse { Time="6:00",Value=30},
                    new Response.DeviceTripsResponse { Time="7:00",Value=80}
                };
                response.Data = tripList;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.DeviceTripsResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DashboardRoute.Route.GetDevicesPeakHours, Name = DashboardRoute.Name.GetDevicesPeakHours)]
        public Entity.BaseResponse<List<Response.DevicePeakHoursResponse>> GetDevicesPeakHours(Request.ElevatorsPeakRequest elevatorIds)
        {
            if (elevatorIds == null)
            {
                return new Entity.BaseResponse<List<Response.DevicePeakHoursResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.DevicePeakHoursResponse>> response = new Entity.BaseResponse<List<Response.DevicePeakHoursResponse>>(true);
            try
            {
                List<Response.DevicePeakHoursResponse> tripList = new List<Response.DevicePeakHoursResponse> {
                new Response.DevicePeakHoursResponse { Time="0:00",Value= new List<Response.PeakLookup>
                {
                    new Response.PeakLookup {Name="e1",Value="10"},
                    new Response.PeakLookup {Name="e2",Value="10"},
                }
                },
                new Response.DevicePeakHoursResponse { Time="1:00",Value= new List<Response.PeakLookup>
                {
                    new Response.PeakLookup {Name="e1",Value="20"},
                    new Response.PeakLookup {Name="e2",Value="20"},
                }
                },
                new Response.DevicePeakHoursResponse { Time="3:00",Value= new List<Response.PeakLookup>
                {
                    new Response.PeakLookup {Name="e1",Value="30"},
                    new Response.PeakLookup {Name="e2",Value="30"},
                }
                },
                };
                response.Data = tripList;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.DevicePeakHoursResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetBuidingDetailOverview, Name = DashboardRoute.Name.GetBuidingDetailOverview)]
        [EnsureGuidParameterAttribute("entityId", "Dashboard")]
        public Entity.BaseResponse<Entity.BuildingOverviewResponse> GetBuidingOverview(string entityId, DateTime currentDate, string timeZone)
        {
            if (entityId == null || Guid.Parse(entityId) == Guid.Empty)
            {
                return new Entity.BaseResponse<Entity.BuildingOverviewResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.BuildingOverviewResponse> response = new Entity.BaseResponse<Entity.BuildingOverviewResponse>(true);
            try
            {

                var result = _entityService.GetBuildingOverviewDetail(Guid.Parse(entityId),currentDate,timeZone);

                response = result;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.BuildingOverviewResponse>(false, ex.Message);
            }
            return response;
        }

        #region Dynamic Dashboard API

        [HttpGet]
        [Route(DashboardRoute.Route.GetMasterWidget, Name = DashboardRoute.Name.GetMasterWidget)]
        public Entity.BaseResponse<List<Entity.MasterWidget>> GetMasterWidget()
        {
            Entity.BaseResponse<List<Entity.MasterWidget>> response = new Entity.BaseResponse<List<Entity.MasterWidget>>(true);
            try
            {
                response.Data = _service.GetMasterWidget();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.MasterWidget>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetMasterWidgetById, Name = DashboardRoute.Name.GetMasterWidgetById)]
        [EnsureGuidParameter("widgetId", "MasterWidget")]
        public Entity.BaseResponse<Entity.MasterWidget> GetMasterWidgetById(string widgetId)
        {
            Entity.BaseResponse<Entity.MasterWidget> response = new Entity.BaseResponse<Entity.MasterWidget>(true);
            try
            {
                response.Data = _service.GetMasterWidgetById(Guid.Parse(widgetId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.MasterWidget>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DashboardRoute.Route.Manage, Name = DashboardRoute.Name.Manage)]
        public Entity.BaseResponse<Entity.MasterWidget> Manage(Entity.MasterWidget request)
        {
            Entity.BaseResponse<Entity.MasterWidget> response = new Entity.BaseResponse<Entity.MasterWidget>(true);
            try
            {
                var result = _service.ManageMasterWidget(request);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.MasterWidget>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(DashboardRoute.Route.DeleteMasterWidget, Name = DashboardRoute.Name.DeleteMasterWidget)]
        [EnsureGuidParameter("id", "MasterWidget")]
        public Entity.BaseResponse<bool> DeleteMasterWidget(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteMasterWidget(Guid.Parse(id));
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
        [Route(DashboardRoute.Route.GetUserWidget, Name = DashboardRoute.Name.GetUserWidget)]
        public Entity.BaseResponse<List<Entity.UserDasboardWidget>> GetUserWidget()
        {
            Entity.BaseResponse<List<Entity.UserDasboardWidget>> response = new Entity.BaseResponse<List<Entity.UserDasboardWidget>>(true);
            try
            {
                response.Data = _service.GetUserWidget();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.UserDasboardWidget>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetUserWidgetById, Name = DashboardRoute.Name.GetUserWidgetById)]
        [EnsureGuidParameter("widgetId", "UserWidget")]
        public Entity.BaseResponse<Entity.UserDasboardWidget> GetUserWidgetById(string widgetId)
        {
            Entity.BaseResponse<Entity.UserDasboardWidget> response = new Entity.BaseResponse<Entity.UserDasboardWidget>(true);
            try
            {
                response.Data = _service.GetUserWidgetById(Guid.Parse(widgetId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.UserDasboardWidget>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DashboardRoute.Route.ManageUserWidget, Name = DashboardRoute.Name.ManageUserWidget)]
        public Entity.BaseResponse<Entity.UserDasboardWidget> ManageUserWidget(Entity.UserDasboardWidget request)
        {
            Entity.BaseResponse<Entity.UserDasboardWidget> response = new Entity.BaseResponse<Entity.UserDasboardWidget>(true);
            try
            {
                var result = _service.ManageUserWidget(request);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.UserDasboardWidget>(false, ex.Message);
            }
            return response;
        }
        [HttpPut]
        [Route(DashboardRoute.Route.DeleteUserWidget, Name = DashboardRoute.Name.DeleteUserWidget)]
        [EnsureGuidParameter("id", "UserWidget")]
        public Entity.BaseResponse<bool> DeleteUserWidget(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteUserWidget(Guid.Parse(id));
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
        #endregion
    }
}