using component.helper;
using component.logger;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;
using iot.solution.common;

namespace iot.solution.service.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardrepository;
        private readonly LogHandler.Logger _logger;
        private readonly IDeviceService _deviceService;
        private readonly IEntityService _entityService;
        private readonly Regex regex = new Regex(@"\s+");
        private readonly IUserDashboardWidgetRepository _userDashboardWidgetRepository;
        private readonly IMasterWidgetRepository _masterWidgetRepository;
        public DashboardService(IDashboardRepository dashboardrepository, LogHandler.Logger logManager, IDeviceService deviceService, IEntityService entityService, IUserDashboardWidgetRepository userDashboardWidgetRepository, IMasterWidgetRepository masterWidgetRepository)
        {
            _dashboardrepository = dashboardrepository;
            _logger = logManager;
            _deviceService = deviceService;
            _entityService = entityService;
            _userDashboardWidgetRepository = userDashboardWidgetRepository;
            _masterWidgetRepository = masterWidgetRepository;
        }

        public Entity.BaseResponse<Entity.BuildingOverviewResponse> GetBuildingOverview(Guid buildingId, string frequency, DateTime currentDate, string timeZone)
        {
            var result = new Entity.BaseResponse<Entity.BuildingOverviewResponse>(true);
            try
            {
                result.Data = _entityService.GetBuildingOverview(buildingId, frequency,currentDate,timeZone);
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

        public Entity.BaseResponse<Entity.DashboardOverviewResponse> GetOverview(DateTime currentDate, string timeZone)
        {
            var result = new Entity.BaseResponse<Entity.DashboardOverviewResponse>(true);
            Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.DashboardOverviewResponse>>();
            try
            {
                listResult = _dashboardrepository.GetStatistics(currentDate, timeZone);
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

        #region Dynamic Dashboard

        private string RemoveWhiteSpace(string str)
        {
            return Regex.Replace(str, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
        }

        #region MasterWidget
        public Entity.ActionStatus ManageMasterWidget(Entity.MasterWidget request)
        {
            try
            {
                var dbMasterWidget = Mapper.Configuration.Mapper.Map<Entity.MasterWidget, Model.MasterWidget>(request);
                Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
                if (request.Guid == null || request.Guid == Guid.Empty)
                {

                    dbMasterWidget.Guid = Guid.NewGuid();
                    dbMasterWidget.Widgets = RemoveWhiteSpace(Convert.ToString(request.Widgets));

                    dbMasterWidget.IsActive = true;
                    dbMasterWidget.IsDeleted = false;
                    dbMasterWidget.CreatedDate = DateTime.Now;
                    dbMasterWidget.CreatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                    actionStatus = _masterWidgetRepository.Insert(dbMasterWidget);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.MasterWidget, Entity.MasterWidget>(actionStatus.Data);

                }
                else
                {
                    var uniqMasterWidget = _masterWidgetRepository.GetByUniqueId(x => x.Guid == dbMasterWidget.Guid);
                    if (uniqMasterWidget == null)
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : MasterWidget");
                    if (uniqMasterWidget != null)
                    {
                        uniqMasterWidget.Widgets = RemoveWhiteSpace(Convert.ToString(request.Widgets));
                        uniqMasterWidget.ModifiedDate = DateTime.Now;
                        uniqMasterWidget.ModifiedBy = component.helper.SolutionConfiguration.CurrentUserId;
                        actionStatus = _masterWidgetRepository.Update(uniqMasterWidget);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.MasterWidget, Entity.MasterWidget>(actionStatus.Data);
                    }
                    else
                    {
                        return new Entity.ActionStatus
                        {
                            Success = false,
                            Message = "Widget name already exists!",
                            Data = null
                        };
                    }
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public List<Entity.MasterWidget> GetMasterWidget()
        {
            try
            {
                return _masterWidgetRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.MasterWidget>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public Entity.MasterWidget GetMasterWidgetById(Guid id)
        {
            try
            {
                return _masterWidgetRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.MasterWidget>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public Entity.ActionStatus DeleteMasterWidget(Guid id)
        {
            try
            {
                //TODO: NEED TO IMPLEMENT RDK CALLS
                var dbUser = _masterWidgetRepository.GetByUniqueId(x => x.Guid == id);
                if (dbUser == null)
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Master Widget");
                dbUser.IsDeleted = true;
                dbUser.ModifiedDate = DateTime.Now;
                dbUser.ModifiedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _masterWidgetRepository.Update(dbUser);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        #endregion

        #region UserWidget
        public Entity.ActionStatus ManageUserWidget(Entity.UserDasboardWidget request)
        {
            try
            {
                if (request.DashboardName.ToLower().Trim().Equals("system default"))
                {
                    return new Entity.ActionStatus
                    {
                        Success = false,
                        Message = "Dashboard Name Can not be System Default !",
                        Data = null
                    };
                }
                Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
                var dbUserWidget = Mapper.Configuration.Mapper.Map<Entity.UserDasboardWidget, Model.UserDasboardWidget>(request);
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    var checkExisting = _userDashboardWidgetRepository.FindBy(x => x.DashboardName.Equals(request.DashboardName) && x.UserId == component.helper.SolutionConfiguration.CurrentUserId && x.IsActive == true && !x.IsDeleted).FirstOrDefault();
                    if (checkExisting == null)
                    {
                        if (request.IsDefault)
                        {
                            var response = _userDashboardWidgetRepository.FindBy(r => r.IsDefault == true && r.UserId == component.helper.SolutionConfiguration.CurrentUserId).Select(p => Mapper.Configuration.Mapper.Map<Model.UserDasboardWidget>(p)).FirstOrDefault();
                            if (response != null)
                            {
                                response.IsDefault = false;
                                _userDashboardWidgetRepository.Update(response);
                            }
                        }
                        string renderedValues = string.Join(",", request.WidgetsList);
                        dbUserWidget.Guid = Guid.NewGuid();
                        dbUserWidget.UserId = component.helper.SolutionConfiguration.CurrentUserId;
                        string res = RemoveWhiteSpace(Convert.ToString(renderedValues));
                        dbUserWidget.Widgets = "[" + res + "]";
                        dbUserWidget.IsActive = true;
                        dbUserWidget.IsDeleted = false;
                        dbUserWidget.CreatedDate = DateTime.Now;
                        dbUserWidget.CreatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                        actionStatus = _userDashboardWidgetRepository.Insert(dbUserWidget);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.UserDasboardWidget, Entity.UserDasboardWidget>(actionStatus.Data);
                    }
                    else
                    {
                        actionStatus.Success = false;
                        actionStatus.Message = "Dashboard Name Already Exists !!";
                    }
                }
                else
                {
                    var uniqUserWidget = _userDashboardWidgetRepository.GetByUniqueId(x => x.Guid == dbUserWidget.Guid);
                    if (uniqUserWidget == null)
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : User Widget");
                    if (!uniqUserWidget.DashboardName.Equals(request.DashboardName.Trim()))
                    {
                        var checkExisting = _userDashboardWidgetRepository.FindBy(x => x.DashboardName.Equals(request.DashboardName) && x.UserId == component.helper.SolutionConfiguration.CurrentUserId && x.IsActive == true && !x.IsDeleted).FirstOrDefault();
                        if (checkExisting != null)
                        {
                            return new Entity.ActionStatus
                            {
                                Success = false,
                                Message = "Widget already exists!",
                                Data = null
                            };
                        }
                    }

                    if (request.IsDefault && !uniqUserWidget.IsDefault)
                    {
                        var response = _userDashboardWidgetRepository.FindBy(r => r.IsDefault == true && r.Guid != request.Guid && r.UserId == component.helper.SolutionConfiguration.CurrentUserId).Select(p => Mapper.Configuration.Mapper.Map<Model.UserDasboardWidget>(p)).FirstOrDefault();
                        if (response != null)
                        {
                            response.IsDefault = false;
                            _userDashboardWidgetRepository.Update(response);
                        }
                    }

                    if (uniqUserWidget != null)
                    {
                        string renderedValues = string.Join(",", request.WidgetsList);
                        string res = RemoveWhiteSpace(Convert.ToString(renderedValues));
                        uniqUserWidget.DashboardName = request.DashboardName;
                        uniqUserWidget.Widgets = "[" + res + "]";
                        uniqUserWidget.IsActive = request.IsActive;
                        uniqUserWidget.IsDefault = request.IsDefault;
                        uniqUserWidget.ModifiedDate = DateTime.Now;
                        uniqUserWidget.ModifiedBy = component.helper.SolutionConfiguration.CurrentUserId;
                        actionStatus = _userDashboardWidgetRepository.Update(uniqUserWidget);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.UserDasboardWidget, Entity.UserDasboardWidget>(actionStatus.Data);
                    }
                    else
                    {
                        return new Entity.ActionStatus
                        {
                            Success = false,
                            Message = "Widget already exists!",
                            Data = null
                        };
                    }
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public List<Entity.UserDasboardWidget> GetUserWidget()
        {
            try
            {
                return _userDashboardWidgetRepository.GetAll().Where(e => !e.IsDeleted && e.UserId == component.helper.SolutionConfiguration.CurrentUserId || e.IsSystemDefault == true).Select(p => Mapper.Configuration.Mapper.Map<Entity.UserDasboardWidget>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public Entity.UserDasboardWidget GetUserWidgetById(Guid id)
        {
            try
            {
                return _userDashboardWidgetRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.UserDasboardWidget>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public Entity.ActionStatus DeleteUserWidget(Guid id)
        {
            try
            {
                //TODO: NEED TO IMPLEMENT RDK CALLS
                var dbUser = _userDashboardWidgetRepository.GetByUniqueId(x => x.Guid == id);
                if (dbUser == null)
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : User Widget");
                dbUser.IsDeleted = true;
                dbUser.ModifiedDate = DateTime.Now;
                dbUser.ModifiedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _userDashboardWidgetRepository.Update(dbUser);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        #endregion

        #endregion

    }
}
