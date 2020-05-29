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
using System.Linq;
using iot.solution.host.Filter;

namespace host.iot.solution.Controllers
{
    [Route(ChartRoute.Route.Global)]
    [ApiController]
    public class ChartController : BaseController
    {
        private readonly IChartService _chartService;
        
        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        }

        [HttpPost]
        [Route(ChartRoute.Route.GetPeakHoursByElevator, Name = ChartRoute.Name.GetPeakHoursByElevator)]
        public Entity.BaseResponse<List<Response.DevicePeakHoursResponse>> PeakHoursByElevator(Request.ElevatorsPeakRequest request)
        {
            Entity.BaseResponse<List<Response.DevicePeakHoursResponse>> response = new Entity.BaseResponse<List<Response.DevicePeakHoursResponse>>();
            try
            {
                response = _chartService.GetPeakHoursByElevator(request);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.DevicePeakHoursResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(ChartRoute.Route.GetOperationHours, Name = ChartRoute.Name.GetOperationHours)]
        public Entity.BaseResponse<List<Response.OperationHours>> GetOperationHours(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.OperationHours>> response = new Entity.BaseResponse<List<Response.OperationHours>>();
            try
            {
                response = _chartService.GetOperationHours(request);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.OperationHours>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(ChartRoute.Route.TripsByElevator, Name = ChartRoute.Name.TripsByElevator)]
        [EnsureGuidParameterAttribute("elevatorId", "Chart")]
        public Entity.BaseResponse<List<Response.DeviceTripsResponse>> TripsByElevator(string elevatorId)
        {
            Entity.BaseResponse<List<Response.DeviceTripsResponse>> response = new Entity.BaseResponse<List<Response.DeviceTripsResponse>>(true);
            try
            {
                response = _chartService.GetTripsByElevator(Guid.Parse(elevatorId));
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.DeviceTripsResponse>>(false, ex.Message);
            }
            return response;
        }
    }
}