using component.helper;
using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
namespace iot.solution.model.Repository.Implementation
{
    public class DashboardRepository:GenericRepository<Models.Entity>,IDashboardRepository
    {
        private readonly LogHandler.Logger _logger;
        public DashboardRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            _logger = logManager;
            _uow = unitOfWork;
        }

        public Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> GetStatistics()
        {
            Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> result = new Entity.BaseResponse<List<Entity.DashboardOverviewResponse>>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "DashboardRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[CompanyStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Entity.DashboardOverviewResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                _logger.InfoLog(Constants.ACTION_EXIT, "DashboardRepository.Get");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
