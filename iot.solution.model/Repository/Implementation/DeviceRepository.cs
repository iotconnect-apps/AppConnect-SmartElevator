using component.logger;
using iot.solution.data;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class DeviceRepository : GenericRepository<Model.Elevator>, IDeviceRepository
    {
        private readonly LogHandler.Logger logger;

        public DeviceRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Model.Elevator Get(string device)
        {
            var result = new Model.Elevator();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.Get");
                _uow.DbContext.Elevator.Where(u => u.Name.Equals(device, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.ActionStatus Manage(Model.Elevator request,bool IsEdit=false)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("templateGuid", request.TemplateGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("uniqueId", request.UniqueId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("name", request.Name, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("note", request.Note, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("tag", request.Tag, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("description", request.Description, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("specification", request.Specification, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("image", request.Image, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isedit", IsEdit, DbType.Boolean, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isProvisioned", request.IsProvisioned, DbType.Boolean, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                   // int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Elevator_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Elevator_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = Guid.Parse(parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString());
                    //string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    //if (!string.IsNullOrEmpty(guidResult))
                    //{
                    //    result.Data = guidResult;
                    //    result.Success = true;
                    //    result.Message = "Elevator Added Succesfully";
                    //}

                    //string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    //if (!string.IsNullOrEmpty(guidResult))
                    //{
                    //    result.Data = _uow.DbContext.Elevator.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();
                    //}
                    //   result.Data = int.Parse(parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString());

                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
                result.Success = false;
            }
            return result;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            throw new NotImplementedException();
        }
        public Entity.SearchResult<List<Entity.Elevator>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.Elevator>> result = new Entity.SearchResult<List<Entity.Elevator>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Elevator_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.Elevator>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.DeviceDetailResponse>> DetailList(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.DeviceDetailResponse>> result = new Entity.SearchResult<List<Entity.DeviceDetailResponse>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.ListByEntity");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.Guid, DbType.Guid, ParameterDirection.Input));                  
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));                 
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_ListByEntity]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.DeviceDetailResponse>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.ListByEntity");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.DeviceSearchResponse>> result = new Entity.SearchResult<List<Entity.DeviceSearchResponse>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.GatewayList");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isParent", true, DbType.Boolean, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.DeviceSearchResponse>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.GatewayList");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Model.Elevator>> GetChildDevice(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Model.Elevator>> result = new Entity.SearchResult<List<Model.Elevator>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "ChildDeviceRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CompanyId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[ChildDevice_ListByGuid]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Model.Elevator>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "ChildDeviceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public List<Response.EntityDevicesResponse> GetEntityDevices(Guid? entityId, Guid? deviceId)
        {
            List<Response.EntityDevicesResponse> result = new List<Response.EntityDevicesResponse>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "GetEntityDevices.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CompanyId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("entityGuid", entityId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("parentDeviceGuid", deviceId, DbType.Guid, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_Lookup]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.EntityDevicesResponse>(dbDataReader, null);
                }
                logger.InfoLog(Constants.ACTION_EXIT, "GetEntityDevices.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.BaseResponse<int> ValidateKit(string kitCode)
        {
            Entity.BaseResponse<int> result = new Entity.BaseResponse<int>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "ValidateKit.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CompanyId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("kitCode", kitCode, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));                    
                    sqlDataAccess.ExecuteScalar(sqlDataAccess.CreateCommand("[Validate_KitCode]", CommandType.StoredProcedure, null), parameters.ToArray());
                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        result.IsSuccess = true;
                    }
                    else {
                        result.IsSuccess = false;
                    }                     
                }
                logger.InfoLog(Constants.ACTION_EXIT, "ValidateKit.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
       public Entity.BaseResponse<List<Entity.HardwareKit>> ProvisionKit(Entity.ProvisionKitRequest request)
        {
            Entity.BaseResponse<List<Entity.HardwareKit>> result = new Entity.BaseResponse<List<Entity.HardwareKit>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.ProvisionKit");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {

                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CompanyId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("kitCode", request.KitCode, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("uniqueId", request.UniqueId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[HardwareKit_GetStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Entity.HardwareKit>(dbDataReader, null);
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.ProvisionKit");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public Entity.BaseResponse<Response.DeviceDetailsResponse> GetDeviceDetail(Guid deviceId)
        {
            Entity.BaseResponse<List<Response.DeviceDetailsResponse>> result = new Entity.BaseResponse<List<Response.DeviceDetailsResponse>>();
            var deviceDetail = new Entity.BaseResponse<Response.DeviceDetailsResponse>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.GetDeviceDetail");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                     List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CompanyId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", deviceId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[ElevatorStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Response.DeviceDetailsResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                    if (result.Data.Count > 0)
                    {
                        deviceDetail.Data = result.Data[0];
                        deviceDetail.LastSyncDate = result.LastSyncDate;
                        deviceDetail.IsSuccess = true;
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.GetDeviceDetail");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return deviceDetail;
        }


        #region Lookup
        public List<Entity.LookupItem> GetGetwayLookup()
        {
            using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
            {
                return sqlDataAccess.QueryList<Entity.LookupItem>("SELECT CONVERT(NVARCHAR(50),[Guid]) AS [Value], [name] AS [Text] FROM [Device] WHERE [parentDeviceGuid] IS NULL AND [isActive] = 1 AND [isDeleted] = 0");
            }
        }
        public List<Entity.LookupItem> GetDeviceLookup()
        {
            using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
            {
                return sqlDataAccess.QueryList<Entity.LookupItem>("SELECT CONVERT(NVARCHAR(50),[Guid]) AS [Value], [name] AS [Text] FROM [Device] WHERE [parentDeviceGuid] IS NOT NULL AND [isActive] = 1 AND [isDeleted] = 0");
            }
        }

        public Entity.BaseResponse<List<Entity.BuildingElevatorLookup>> GetDeviceLookupByEntityId(Guid buildingId)
        {
            Entity.BaseResponse<List<Entity.BuildingElevatorLookup>> result = new Entity.BaseResponse<List<Entity.BuildingElevatorLookup>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.ElevatorLookupByBuilding");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {

                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CompanyId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("buildingGuid", buildingId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Elevator_Lookup]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Entity.BuildingElevatorLookup>(dbDataReader, null);
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.ProvisionKit");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        #endregion
    }
}
