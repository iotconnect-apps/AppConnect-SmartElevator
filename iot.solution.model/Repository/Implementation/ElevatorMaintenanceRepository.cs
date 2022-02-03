using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class ElevatorMaintenanceRepository : GenericRepository<Model.ElevatorMaintenance>, IElevatorMaintenanceRepository
    {
        private readonly LogHandler.Logger logger;
        public ElevatorMaintenanceRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Entity.ElevatorMaintenance Get(Guid id, DateTime currentDate, string timeZone)
        {
            List<Entity.ElevatorMaintenance> result = new List<Entity.ElevatorMaintenance>();
            Entity.ElevatorMaintenance maintenanceDetail = new Entity.ElevatorMaintenance();
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                {
                    dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                }
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceMaintenanceRepository.GetMaintenence");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, "v1");
                    parameters.Add(sqlDataAccess.CreateParameter("guid", id, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[ElevatorMaintenance_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Entity.ElevatorMaintenance>(dbDataReader, null);
                    if (result.Count > 0)
                    {
                        maintenanceDetail = result[0];
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceMaintenanceRepository.GetUpComingList");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return maintenanceDetail;
        }

        public List<Entity.ElevatorMaintenanceResponse> GetUpComingList(Entity.ElevatorMaintenanceRequest request)
        {
            List<Entity.ElevatorMaintenanceResponse> result = new List<Entity.ElevatorMaintenanceResponse>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "ElevatorMaintenanceRepository.GetUpComingList");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;

                    if (DateTime.TryParse(request.CurrentDate.Value.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, "v1");
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    if(request.BuildingGuid.HasValue)
                    parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.BuildingGuid, DbType.Guid, ParameterDirection.Input));
                    if (request.ElevatorGuid.HasValue)
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.ElevatorGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[ElevatorMaintenance_UpComingList]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Entity.ElevatorMaintenanceResponse>(dbDataReader, null);
                 
                }
                logger.InfoLog(Constants.ACTION_EXIT, "ElevatorMaintenanceRepository.GetUpComingList");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>> result = new Entity.SearchResult<List<Entity.ElevatorMaintenanceDetail>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "ElevatorMaintenanceRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(request.CurrentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    if (!request.EntityId.Equals(Guid.Empty))
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityId, DbType.Guid, ParameterDirection.Input));
                    }
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[ElevatorMaintenance_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.ElevatorMaintenanceDetail>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "ElevatorMaintenanceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.ActionStatus Manage(Model.ElevatorMaintenance request)
        {
            Entity.ActionStatus result = new Entity.ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "ElevatorMaintenanceRepository.Manage");
                int outPut = 0;
                int intResult = 0;
                string guidResult = string.Empty;
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("description", request.Description, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("startDateTime", request.StartDateTime, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("endDateTime", request.EndDateTime, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    if (request.Guid == null || request.Guid == Guid.Empty)
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                        parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityGuid, DbType.Guid, ParameterDirection.Input));
                        parameters.Add(sqlDataAccess.CreateParameter("elevatorGuid", request.ElevatorGuid, DbType.Guid, ParameterDirection.Input));
                        intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[ElevatorMaintenance_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                        guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    }
                    else
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                        intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[ElevatorMaintenance_UpdateStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                        guidResult = request.Guid.ToString();
                    }
                    outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        if (!string.IsNullOrEmpty(guidResult))
                        {
                            result.Data = Guid.Parse(guidResult);
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "ElevatorMaintenanceRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}
