using component.helper;
using component.logger;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;

namespace iot.solution.service.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardrepository;
        private readonly LogHandler.Logger _logger;
        private readonly IDeviceService _deviceService;
        private readonly IEntityService _entityService;
        public DashboardService(IDashboardRepository dashboardrepository, LogHandler.Logger logManager, IDeviceService deviceService, IEntityService entityService)
        {
            _dashboardrepository = dashboardrepository;
            _logger = logManager;
            _deviceService = deviceService;
            _entityService = entityService;
        }

        public Entity.BaseResponse<Entity.BuildingOverviewResponse> GetBuildingOverview(Guid buildingId, string frequency)
        {
            var result = new Entity.BaseResponse<Entity.BuildingOverviewResponse>(true);
            try
            {
                result.Data = _entityService.GetBuildingOverview(buildingId, frequency);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
                return new Entity.BaseResponse<Entity.BuildingOverviewResponse>(false, ex.Message);
            }
            return result;
        }

        public List<Entity.LookupItem> GetBuildingsLookup()
        {
            List<Entity.LookupItem> lstResult = new List<Entity.LookupItem>();
            try
            {
                lstResult = (from g in _dashboardrepository.FindBy(r => r.CompanyGuid == SolutionConfiguration.CompanyId)
                             select new Entity.LookupItem()
                             {
                                 Text = g.Name,
                                 Value = g.Guid.ToString().ToUpper()
                             }).ToList();
            }
            catch (Exception ex)
            {

                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return lstResult;


        }

        public Entity.BaseResponse<Entity.DashboardOverviewResponse> GetOverview()
        {
            var result = new Entity.BaseResponse<Entity.DashboardOverviewResponse>(true);
            Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.DashboardOverviewResponse>>();
            try
            {
                listResult = _dashboardrepository.GetStatistics();
                if (listResult.Data.Count > 0)
                {
                    result.Data = listResult.Data[0];
                    result.Data.Alerts = new Dictionary<string, string>();
                    result.Data.Alerts.Add("Critical", result.Data.CriticalAlertCount.ToString());
                    result.Data.Alerts.Add("Information", result.Data.InformationAlertCount.ToString());
                    result.Data.Alerts.Add("Major", result.Data.MajorAlertCount.ToString());
                    result.Data.Alerts.Add("Minor", result.Data.MinorAlertCount.ToString());
                    result.Data.Alerts.Add("Warning", result.Data.WarningAlertCount.ToString());
                    result.LastSyncDate = listResult.LastSyncDate;
                }

                var deviceResult = _deviceService.GetDeviceCounters();
                if (deviceResult.IsSuccess && deviceResult.Data != null)
                {
                    result.Data.TotalDisconnectedElevators = deviceResult.Data.disConnected.ToString();
                    result.Data.TotalConnectedElevators = deviceResult.Data.connected.ToString();
                    result.Data.TotalElevators = deviceResult.Data.total.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
                return new Entity.BaseResponse<Entity.DashboardOverviewResponse>(false);
            }
        }

    }
}
