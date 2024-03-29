﻿using component.helper;
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
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class EntityRepository : GenericRepository<Model.Entity>, IEntityRepository
    {
        private readonly LogHandler.Logger logger;
        public EntityRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
        public List<Entity.LookupItem> GetLookup(Guid companyId)
        {
            var result = new List<Entity.LookupItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "EntityRepository.GetLookup");
                result = _uow.DbContext.Entity.Where(u => u.CompanyGuid.Equals(companyId) && !u.Guid.Equals(component.helper.SolutionConfiguration.EntityGuid) && u.ParentEntityGuid.Equals(SolutionConfiguration.EntityGuid) && u.IsActive == true && !u.IsDeleted).Select(g => new Entity.LookupItem() { Text = g.Name, Value = g.Guid.ToString() }).ToList();
                logger.InfoLog(Constants.ACTION_EXIT, "EntityRepository.GetLookup");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public List<Entity.LookupItem> GetWingLookup(Guid companyId)
        {
            var result = new List<Entity.LookupItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "EntityRepository.GetWingLookup");
                result = _uow.DbContext.Entity.Where(u => u.CompanyGuid.Equals(companyId) && !u.ParentEntityGuid.Equals(SolutionConfiguration.EntityGuid) && u.IsActive == true && !u.IsDeleted).Select(g => new Entity.LookupItem() { Text = g.Name, Value = g.Guid.ToString() }).ToList();
                logger.InfoLog(Constants.ACTION_EXIT, "EntityRepository.GetWingLookup");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.EntityDetail>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.EntityDetail>> result = new Entity.SearchResult<List<Entity.EntityDetail>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "EntityRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    if (!request.EntityId.Equals(Guid.Empty) && !request.EntityId.Equals(SolutionConfiguration.EntityGuid))
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("parentEntityGuid", request.EntityId, DbType.Guid, ParameterDirection.Input));
                    }
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Entity_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.EntityDetail>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "EntityRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus Manage(Model.Entity request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "EntityRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("parentEntityGuid", request.ParentEntityGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("name", request.Name, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("description", request.Description, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("address", request.Address, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("address2", request.Address2, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("city", request.City, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("stateGuid", request.StateGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("countryGuid", request.CountryGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("zipCode", request.Zipcode, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("latitude", request.Latitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("longitude", request.Longitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("image", request.Image, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Entity_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        if (!string.IsNullOrEmpty(guidResult))
                        {
                            result.Data = _uow.DbContext.Entity.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();
                        }
                    }
                    else
                    {
                        result.Message = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "EntityRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.BaseResponse<List<Entity.BuildingOverviewResponse>> GetBuildingOverview(Guid buildingId, string frequency, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse <List<Entity.BuildingOverviewResponse>> result = new Entity.BaseResponse<List<Entity.BuildingOverviewResponse>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "EntityRepository.GetBuildingOverview");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", buildingId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("frequency", frequency, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[BuildingStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Entity.BuildingOverviewResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "EntityRepository.GetBuildingOverview");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
