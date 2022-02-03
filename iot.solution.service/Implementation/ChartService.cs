using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using LogHandler = component.services.loghandler;
using Request = iot.solution.entity.Request;
using Response = iot.solution.entity.Response;
using Entity = iot.solution.entity;
using component.helper.Interface;
namespace iot.solution.service.Implementation
{
    public class ChartService : IChartService
    {
        private readonly IEntityRepository _entityRepository;
        private readonly LogHandler.Logger _logger;
        private readonly IEmailHelper _emailHelper;
        public string ConnectionString = component.helper.SolutionConfiguration.Configuration.ConnectionString;
        //private readonly LogHandler.Logger _logger;
        public ChartService(IEntityRepository entityRepository, LogHandler.Logger logger, IEmailHelper emailHelper)//, LogHandler.Logger logger)
        {
            _entityRepository = entityRepository;
           _logger = logger;
            _emailHelper = emailHelper;
        }


        public Entity.ActionStatus TelemetrySummary_DayWise()
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[TelemetrySummary_DayWise_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus TelemetrySummary_HourWise()
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[TelemetrySummary_HourWise_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.BaseResponse<List<Response.DevicePeakHoursResponse>> GetPeakHoursByElevator(Request.ElevatorsPeakRequest request)
        {

            Entity.BaseResponse<List<Response.DevicePeakHoursResponse>> responseResult = new Entity.BaseResponse<List<Response.DevicePeakHoursResponse>>();
            List<Response.DevicePeakHoursResponse> tripList = new List<Response.DevicePeakHoursResponse>();
            try
            {
                string ids = String.Join(",", request.ElevatorList);
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_PeakHoursByElevator.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<Response.OperationHours> result = new List<Response.OperationHours>();
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("elevators", ids, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("frequency", request.Frequency, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_PeakHoursByElevator]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.OperationHours>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        responseResult.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }

                    var resultGrp = from data in result
                                    group data by data.Name
                                  into egroup
                                 select egroup;
                    
                    foreach (var group in resultGrp)
                    {
                        List<Response.PeakLookup> lookupItems = new List<Response.PeakLookup>();
                        foreach (var items in group)
                        {
                            lookupItems.Add(new Response.PeakLookup { Name = items.ElevatorName, Value = items.Value });
                        }
                        tripList.Add(new Response.DevicePeakHoursResponse { Time = group.Key, Value = lookupItems });
                    }
                    responseResult.Data = tripList;
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return responseResult;
        }

        public Entity.BaseResponse<List<Response.DeviceTripsResponse>> GetTripsByElevator(Guid elevatorId)
        {
            Entity.BaseResponse<List<Response.DeviceTripsResponse>> result = new Entity.BaseResponse<List<Response.DeviceTripsResponse>>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_TripsByElevator.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("guid", elevatorId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_TripsByElevator]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Response.DeviceTripsResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
               _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return result;

        }

        public Entity.BaseResponse<List<Response.OperationHours>> GetOperationHours(Request.ChartRequest request)
        {
            Entity.BaseResponse <List<Response.OperationHours>> result = new Entity.BaseResponse<List<Response.OperationHours>>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_OperationHours.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.BuildingId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("frequency", request.Frequency, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_OperationHours]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Response.OperationHours>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Entity.ActionStatus SendSubscriptionNotification()
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Get_UserSubscriptionEndData]", CommandType.StoredProcedure, null), parameters.ToArray());
                    var result = DataUtils.DataReaderToList<Response.SubscriptionEndData>(dbDataReader, null);
                    foreach (var item in result)
                    {
                        _emailHelper.SendSubscriptionOverEmail(item.CustomerName, item.ExpiryDate.ToString("dd MMM yyy"), item.Email);
                    }
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
    }
}
